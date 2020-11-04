using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using JetBrains.Annotations;

using MetaBrainz.Common.Json;
using MetaBrainz.ListenBrainz.Interfaces;
using MetaBrainz.ListenBrainz.Json;
using MetaBrainz.ListenBrainz.Objects;

namespace MetaBrainz.ListenBrainz {

  /// <summary>Main class for accessing the ListenBrainz API.</summary>
  [PublicAPI]
  public sealed class ListenBrainz : IDisposable {

    #region Constants

    /// <summary>The default number of listens returned in a single GET request.</summary>
    public const int DefaultItemsPerGet = 25;

    /// <summary>The default time range for a request for listen data, in sets of 5 days.</summary>
    public const int DefaultTimeRange = 3;

    /// <summary>The maximum number of listens returned in a single GET request.</summary>
    public const int MaxItemsPerGet = 100;

    /// <summary>Maximum overall listen size in bytes, to prevent egregious spamming.</summary>
    public const int MaxListenSize = 10240;

    /// <summary>The maximum length of a tag.</summary>
    public const int MaxTagLength = 64;

    /// <summary>The maximum number of tags per listen.</summary>
    public const int MaxTagsPerListen = 50;

    /// <summary>The maximum time range for a request for listen data, in sets of 5 days.</summary>
    public const int MaxTimeRange = 73;

    /// <summary>The URL included in the user agent for requests as part of this library's information.</summary>
    public const string UserAgentUrl = "https://github.com/Zastai/ListenBrainz";

    /// <summary>The root location of the web service.</summary>
    public const string WebServiceRoot = "/1/";

    #endregion

    #region Static Fields / Properties

    /// <summary>
    /// The default contact info portion of the user agent to use for requests; used as initial value for <see cref="ContactInfo"/>.
    /// </summary>
    public static Uri? DefaultContactInfo { get; set; }

    /// <summary>The default port number to use for requests (-1 to not specify any explicit port).</summary>
    public static int DefaultPort { get; set; } = -1;

    /// <summary>
    /// The default product info portion of the user agent to use for requests; used as initial value for <see cref="ProductInfo"/>.
    /// </summary>
    public static ProductHeaderValue? DefaultProductInfo { get; set; }

    /// <summary>The default server to use for requests.</summary>
    public static string DefaultServer { get; set; } = "api.listenbrainz.org";

    /// <summary>The default internet access protocol to use for requests.</summary>
    public static string DefaultUrlScheme { get; set; } = "https";

    /// <summary>The default user token to use for requests; used as initial value for <see cref="UserToken"/>.</summary>
    public static string? DefaultUserToken { get; set; }

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new ListenBrainz API client instance.
    /// User agent information must have been set up via <see cref="DefaultContactInfo"/> and <see cref="DefaultProductInfo"/>.
    /// </summary>
    public ListenBrainz()
    : this(ListenBrainz.DefaultProductInfo ?? throw new ArgumentNullException(nameof(ListenBrainz.DefaultProductInfo)),
           ListenBrainz.DefaultContactInfo ?? throw new ArgumentNullException(nameof(ListenBrainz.DefaultContactInfo)))
    { }

    /// <summary>
    /// Initializes a new ListenBrainz API client instance.
    /// Contact information must have been set up via <see cref="DefaultContactInfo"/>.
    /// </summary>
    /// <param name="product">The product info portion of the user agent to use for requests.</param>
    public ListenBrainz(ProductHeaderValue product)
    : this(product, ListenBrainz.DefaultContactInfo ?? throw new ArgumentNullException(nameof(ListenBrainz.DefaultContactInfo)))
    { }

    /// <summary>Initializes a new ListenBrainz API client instance.</summary>
    /// <param name="product">The product info portion of the user agent to use for requests.</param>
    /// <param name="contact">
    /// The contact info portion (typically a URL or email address) of the user agent to use for requests.
    /// </param>
    public ListenBrainz(ProductHeaderValue product, Uri contact) {
      this.ContactInfo      = contact;
      this.ProductInfo      = product;
      this.UserAgentContact = new ProductInfoHeaderValue($"({contact})");
      this.UserAgentProduct = new ProductInfoHeaderValue(product);
      this.UserToken        = ListenBrainz.DefaultUserToken;
    }

    /// <summary>Initializes a new ListenBrainz API client instance.</summary>
    /// <param name="product">The product info portion of the user agent to use for requests.</param>
    /// <param name="contact">
    /// The contact info portion (typically a URL or email address) of the user agent to use for requests. Must be a valid URI.
    /// </param>
    public ListenBrainz(ProductHeaderValue product, string contact)
    : this(product, new Uri(contact))
    { }

    /// <summary>
    /// Initializes a new ListenBrainz API client instance.
    /// Product information must have been set up via <see cref="DefaultProductInfo"/>.
    /// </summary>
    /// <param name="contact">
    /// The contact info portion (typically a URL or email address) of the user agent to use for requests.
    /// </param>
    public ListenBrainz(Uri contact)
    : this(ListenBrainz.DefaultProductInfo ?? throw new ArgumentNullException(nameof(ListenBrainz.DefaultProductInfo)), contact)
    { }

    /// <summary>
    /// Initializes a new ListenBrainz API client instance.
    /// Product information must have been set up via <see cref="DefaultProductInfo"/>.
    /// </summary>
    /// <param name="contact">
    /// The contact info portion (typically a URL or email address) of the user agent to use for requests. Must be a valid URI.
    /// </param>
    public ListenBrainz(string contact)
    : this(new Uri(contact))
    { }

    /// <summary>
    /// Initializes a new ListenBrainz API client instance.
    /// Contact information must have been set up via <see cref="DefaultContactInfo"/>.
    /// </summary>
    /// <param name="application">The application name for the product info portion of the user agent to use for requests.</param>
    /// <param name="version">The version number for the product info portion of the user agent to use for requests.</param>
    public ListenBrainz(string application, Version version)
    : this(application, version.ToString())
    { }

    /// <summary>
    /// Initializes a new ListenBrainz API client instance.
    /// Contact information must have been set up via <see cref="DefaultContactInfo"/>.
    /// </summary>
    /// <param name="application">The application name for the product info portion of the user agent to use for requests.</param>
    /// <param name="version">The version number for the product info portion of the user agent to use for requests.</param>
    public ListenBrainz(string application, string version)
    : this(new ProductHeaderValue(application, version))
    { }

    /// <summary>Initializes a new ListenBrainz API client instance.</summary>
    /// <param name="application">The application name for the product info portion of the user agent to use for requests.</param>
    /// <param name="version">The version number for the product info portion of the user agent to use for requests.</param>
    /// <param name="contact">
    /// The contact info portion (typically a URL or email address) of the user agent to use for requests.
    /// </param>
    public ListenBrainz(string application, Version version, Uri contact)
    : this(application, version.ToString(), contact)
    { }

    /// <summary>Initializes a new ListenBrainz API client instance.</summary>
    /// <param name="application">The application name for the product info portion of the user agent to use for requests.</param>
    /// <param name="version">The version number for the product info portion of the user agent to use for requests.</param>
    /// <param name="contact">
    /// The contact info portion (typically a URL or email address) of the user agent to use for requests. Must be a valid URI.
    /// </param>
    public ListenBrainz(string application, Version version, string contact)
    : this(application, version.ToString(), new Uri(contact))
    { }

    /// <summary>Initializes a new ListenBrainz API client instance.</summary>
    /// <param name="application">The application name for the product info portion of the user agent to use for requests.</param>
    /// <param name="version">The version number for the product info portion of the user agent to use for requests.</param>
    /// <param name="contact">
    /// The contact info portion (typically a URL or email address) of the user agent to use for requests.
    /// </param>
    public ListenBrainz(string application, string version, Uri contact)
    : this(new ProductHeaderValue(application, version), contact)
    { }

    /// <summary>Initializes a new ListenBrainz API client instance.</summary>
    /// <param name="application">The application name to use in the User-Agent property for all requests.</param>
    /// <param name="version">The version number to use in the User-Agent property for all requests.</param>
    /// <param name="contact">
    /// The contact info portion (typically a URL or email address) of the user agent to use for requests. Must be a valid URI.
    /// </param>
    public ListenBrainz(string application, string version, string contact)
    : this(application, version, new Uri(contact))
    { }

    #endregion

    #region Public Instance Fields / Properties

    /// <summary>The base URI for all requests.</summary>
    public Uri BaseUri => new UriBuilder(this.UrlScheme, this.Server, this.Port, ListenBrainz.WebServiceRoot).Uri;

    /// <summary>The contact information portion of the user agent to use for requests.</summary>
    public Uri ContactInfo { get; }

    /// <summary>
    /// The port number to use for requests (-1 to not specify any explicit port).<br/>
    /// Changes to this property only take effect when creating the underlying web service client. If this property is set after
    /// requests have been issued, <see cref="Close()"/> must be called for the changes to take effect.
    /// </summary>
    public int Port { get; set; } = ListenBrainz.DefaultPort;

    /// <summary>The product information portion of the user agent to use for requests.</summary>
    public ProductHeaderValue ProductInfo { get; }

    /// <summary>
    /// The server to use for requests.<br/>
    /// Changes to this property only take effect when creating the underlying web service client. If this property is set after
    /// requests have been issued, <see cref="Close()"/> must be called for the changes to take effect.
    /// </summary>
    public string Server { get; set; } = ListenBrainz.DefaultServer;

    /// <summary>
    /// The internet access protocol to use for requests.<br/>
    /// Changes to this property only take effect when creating the underlying web service client. If this property is set after
    /// requests have been issued, <see cref="Close()"/> must be called for the changes to take effect.
    /// </summary>
    public string UrlScheme { get; set; } = ListenBrainz.DefaultUrlScheme;

    /// <summary>
    /// The user token to use for requests.<br/>
    /// For modifications, this must be the token for the user whose data is modified.<br/>
    /// For data retrieval, while this must be a <em>valid</em> user token, it does not need to be that of the user whose data is
    /// requested. Not setting this (or setting it to <see langword="null"/>) is also valid for such scenarios, but will be subject
    /// to stricter rate limiting.
    /// </summary>
    /// <remarks>
    /// Users can find their token on their profile page:
    /// <a href="https://listenbrainz.org/profile/">https://listenbrainz.org/profile/</a>.
    /// </remarks>
    public string? UserToken {
      get => this.Authentication?.Parameter;
      set => this.Authentication = new AuthenticationHeaderValue("Token", value);
    }

    #endregion

    #region Public API

    /// <summary>Information about the active rate limiting. Gets refreshed after every API call.</summary>
    public RateLimitInfo RateLimitInfo { get; private set; }

    #region /1/latest-import

    private static Dictionary<string, string> OptionsForLatestImport(string user) {
      return new Dictionary<string, string> {
        ["user_name"] = user
      };
    }

    /// <summary>Get the timestamp of the newest listen submitted by a user in previous imports to ListenBrainz.</summary>
    /// <param name="user">The MusicBrainz ID of the user whose data is requested.</param>
    /// <returns>An object providing the user's ID and latest import timestamp.</returns>
    /// <remarks>This will access the <c>GET /1/latest-import</c> endpoint.</remarks>
    public ILatestImport GetLatestImport(string user)
      => ListenBrainz.ResultOf(this.GetLatestImportAsync(user));

    /// <summary>Get the timestamp of the newest listen submitted by a user in previous imports to ListenBrainz.</summary>
    /// <param name="user">The MusicBrainz ID of the user whose data is requested.</param>
    /// <returns>An object providing the user's ID and latest import timestamp.</returns>
    /// <remarks>This will access the <c>GET /1/latest-import</c> endpoint.</remarks>
    public async Task<ILatestImport> GetLatestImportAsync(string user)
      => await this.GetAsync<ILatestImport, LatestImport>("latest-import", ListenBrainz.OptionsForLatestImport(user));

    /// <summary>Set the timestamp of the newest listen submitted by a user in previous imports to ListenBrainz.</summary>
    /// <param name="user">The MusicBrainz ID of the user whose data should be modified.</param>
    /// <param name="timestamp">The timestamp to set.</param>
    /// <remarks>
    /// This will access the <c>POST /1/latest-import</c> endpoint and requires <see cref="UserToken"/> to be set to the token
    /// for <paramref name="user"/>.
    /// </remarks>
    public void SetLatestImport(string user, DateTimeOffset timestamp)
      => ListenBrainz.ResultOf(this.SetLatestImportAsync(user, UnixTime.Convert(timestamp)));

    /// <summary>Set the timestamp of the newest listen submitted by a user in previous imports to ListenBrainz.</summary>
    /// <param name="user">The MusicBrainz ID of the user whose data is needed.</param>
    /// <param name="timestamp">
    /// The timestamp to set, expressed as the number of seconds since <see cref="UnixTime.Epoch">the Unix time epoch</see>.
    /// </param>
    /// <remarks>
    /// This will access the <c>POST /1/latest-import</c> endpoint and requires <see cref="UserToken"/> to be set to the token
    /// for <paramref name="user"/>.
    /// </remarks>
    public void SetLatestImport(string user, long timestamp)
      => ListenBrainz.ResultOf(this.SetLatestImportAsync(user, timestamp));

    /// <summary>Set the timestamp of the newest listen submitted by a user in previous imports to ListenBrainz.</summary>
    /// <param name="user">The MusicBrainz ID of the user whose data should be modified.</param>
    /// <param name="timestamp">The timestamp to set.</param>
    /// <return>A task that will perform the operation.</return>
    /// <remarks>
    /// This will access the <c>POST /1/latest-import</c> endpoint.<br/>
    /// Users can find their token on their profile page:
    /// <a href="https://listenbrainz.org/profile/">https://listenbrainz.org/profile/</a>.
    /// </remarks>
    public Task SetLatestImportAsync(string user, DateTimeOffset timestamp)
      => this.SetLatestImportAsync(user, UnixTime.Convert(timestamp));

    /// <summary>Set the timestamp of the newest listen submitted by a user in previous imports to ListenBrainz.</summary>
    /// <param name="user">The MusicBrainz ID of the user whose data is needed.</param>
    /// <param name="timestamp">
    /// The timestamp to set, expressed as the number of seconds since <see cref="UnixTime.Epoch">the Unix time epoch</see>.
    /// </param>
    /// <return>A task that will perform the operation.</return>
    /// <remarks>
    /// This will access the <c>POST /1/latest-import</c> endpoint.<br/>
    /// Users can find their token on their profile page:
    /// <a href="https://listenbrainz.org/profile/">https://listenbrainz.org/profile/</a>.
    /// </remarks>
    public async Task SetLatestImportAsync(string user, long timestamp)
      => await this.PostAsync("latest-import", $"{{ ts: {timestamp} }}", ListenBrainz.OptionsForLatestImport(user));

    #endregion

    #region /1/stats

    private static IDictionary<string, string> OptionsForGetStatistics(int? count, int? offset, StatisticsRange? range) {
      var options = new Dictionary<string, string>(2);
      if (count.HasValue)
        options.Add("count", count.Value.ToString(CultureInfo.InvariantCulture));
      if (offset.HasValue)
        options.Add("offset", offset.Value.ToString(CultureInfo.InvariantCulture));
      if (range.HasValue)
        options.Add("range", range.Value.ToJson());
      return options;
    }

    #region /1/stats/sitewide

    #region /1/stats/sitewide/artists

    #endregion

    #endregion

    #region /1/stats/user/xxx

    #region /1/stats/user/xxx/artist-map

    #endregion

    #region /1/stats/user/xxx/artists

    /// <summary>Gets statistics about a user's most listened-to artists.</summary>
    /// <param name="user">The user for whom the statistics are requested.</param>
    /// <param name="count">
    /// The (maximum) number of entries to return. If not specified (or <see langword="null"/>), all available information will be
    /// returned.
    /// </param>
    /// <param name="offset">
    /// The offset (from the start of the results) of the statistics to return. If not specified (or specified as zero or
    /// <see langword="null"/>), the top most listened-to artists will be returned.
    /// </param>
    /// <param name="range">The range of data to include in the statistics.</param>
    /// <returns>
    /// The requested artist statistics, or <see langword="null"/> if statistics have not yet been computed for the user.
    /// </returns>
    public IUserArtistStatistics? GetArtistStatistics(string user, int? count = null, int? offset = null, StatisticsRange? range = null)
      => ListenBrainz.ResultOf(this.GetArtistStatisticsAsync(user, count, offset, range));

    /// <summary>Gets statistics about a user's most listened-to artists.</summary>
    /// <param name="user">The user for whom the statistics are requested.</param>
    /// <param name="count">
    /// The (maximum) number of entries to return. If not specified (or <see langword="null"/>), all available information will be
    /// returned.
    /// </param>
    /// <param name="offset">
    /// The offset (from the start of the results) of the statistics to return. If not specified (or specified as zero or
    /// <see langword="null"/>), the top most listened-to artists will be returned.
    /// </param>
    /// <param name="range">The range of data to include in the statistics.</param>
    /// <returns>
    /// The requested artist statistics, or <see langword="null"/> if statistics have not yet been computed for the user.
    /// </returns>
    public async Task<IUserArtistStatistics?> GetArtistStatisticsAsync(string user, int? count = null, int? offset = null, StatisticsRange? range = null) {
      var options = ListenBrainz.OptionsForGetStatistics(count, offset, range);
      var task = this.GetOptionalAsync<IUserArtistStatistics, UserArtistStatistics>($"stats/user/{user}/artists", options);
      return await task.ConfigureAwait(false);
    }

    #endregion

    #region /1/stats/user/xxx/daily-activity

    #endregion

    #region /1/stats/user/xxx/listening-activity

    #endregion

    #region /1/stats/user/xxx/recordings

    /// <summary>Gets statistics about a user's most listened-to recordings ("tracks").</summary>
    /// <param name="user">The user for whom the statistics are requested.</param>
    /// <param name="count">
    /// The (maximum) number of entries to return. If not specified (or <see langword="null"/>), all available information will be
    /// returned.
    /// </param>
    /// <param name="offset">
    /// The offset (from the start of the results) of the statistics to return. If not specified (or specified as zero or
    /// <see langword="null"/>), the top most listened-to recordings will be returned.
    /// </param>
    /// <param name="range">The range of data to include in the statistics.</param>
    /// <returns>
    /// The requested recording statistics, or <see langword="null"/> if statistics have not yet been computed for the user.
    /// </returns>
    public IUserRecordingStatistics? GetRecordingStatistics(string user, int? count = null, int? offset = null, StatisticsRange? range = null)
      => ListenBrainz.ResultOf(this.GetRecordingStatisticsAsync(user, count, offset, range));

    /// <summary>Gets statistics about a user's most listened-to recordings ("tracks").</summary>
    /// <param name="user">The user for whom the statistics are requested.</param>
    /// <param name="count">
    /// The (maximum) number of entries to return. If not specified (or <see langword="null"/>), all available information will be
    /// returned.
    /// </param>
    /// <param name="offset">
    /// The offset (from the start of the results) of the statistics to return. If not specified (or specified as zero or
    /// <see langword="null"/>), the top most listened-to recordings will be returned.
    /// </param>
    /// <param name="range">The range of data to include in the statistics.</param>
    /// <returns>
    /// The requested recording statistics, or <see langword="null"/> if statistics have not yet been computed for the user.
    /// </returns>
    public async Task<IUserRecordingStatistics?> GetRecordingStatisticsAsync(string user, int? count = null, int? offset = null, StatisticsRange? range = null) {
      var options = ListenBrainz.OptionsForGetStatistics(count, offset, range);
      var task = this.GetOptionalAsync<IUserRecordingStatistics, UserRecordingStatistics>($"stats/user/{user}/recordings", options);
      return await task.ConfigureAwait(false);
    }

    #endregion

    #region /1/stats/user/xxx/releases

    /// <summary>Gets statistics about a user's most listened-to releases ("albums").</summary>
    /// <param name="user">The user for whom the statistics are requested.</param>
    /// <param name="count">
    /// The (maximum) number of entries to return. If not specified (or <see langword="null"/>), all available information will be
    /// returned.
    /// </param>
    /// <param name="offset">
    /// The offset (from the start of the results) of the statistics to return. If not specified (or specified as zero or
    /// <see langword="null"/>), the top most listened-to releases will be returned.
    /// </param>
    /// <param name="range">The range of data to include in the statistics.</param>
    /// <returns>
    /// The requested release statistics, or <see langword="null"/> if statistics have not yet been computed for the user.
    /// </returns>
    public IUserReleaseStatistics? GetReleaseStatistics(string user, int? count = null, int? offset = null, StatisticsRange? range = null)
      => ListenBrainz.ResultOf(this.GetReleaseStatisticsAsync(user, count, offset, range));

    /// <summary>Gets statistics about a user's most listened-to releases ("albums").</summary>
    /// <param name="user">The user for whom the statistics are requested.</param>
    /// <param name="count">
    /// The (maximum) number of entries to return. If not specified (or <see langword="null"/>), all available information will be
    /// returned.
    /// </param>
    /// <param name="offset">
    /// The offset (from the start of the results) of the statistics to return. If not specified (or specified as zero or
    /// <see langword="null"/>), the top most listened-to releases will be returned.
    /// </param>
    /// <param name="range">The range of data to include in the statistics.</param>
    /// <returns>
    /// The requested releases statistics, or <see langword="null"/> if statistics have not yet been computed for the user.
    /// </returns>
    public async Task<IUserReleaseStatistics?> GetReleaseStatisticsAsync(string user, int? count = null, int? offset = null, StatisticsRange? range = null) {
      var options = ListenBrainz.OptionsForGetStatistics(count, offset, range);
      var task = this.GetOptionalAsync<IUserReleaseStatistics, UserReleaseStatistics>($"stats/user/{user}/releases", options);
      return await task.ConfigureAwait(false);
    }

    #endregion

    #endregion

    #endregion

    #region /1/submit-listens

    private ConfiguredTaskAwaitable SubmitListensAsync(SubmissionPayload payload)
      => this.PostAsync("submit-listens", payload).ConfigureAwait(false);

    private ConfiguredTaskAwaitable SubmitListensAsync(string payload)
      => this.PostAsync("submit-listens", payload).ConfigureAwait(false);

    #region Import Listens

    /// <summary>The maximum number of listens that can fit into a single API request.</summary>
    /// <remarks>
    /// A JSON listen payload contains:
    /// <list type="bullet">
    /// <item>
    ///   <term>a minimum of 37 characters of fixed overhead: </term>
    ///   <description><c>{"listen_type":"import","payload":[]}</c></description>
    /// </item>
    /// <item>
    ///   <term>a minimum of 71 characters for the listen data: </term>
    ///   <description><c>{"listened_at":0,"track_metadata":{"artist_name":"?","track_name":"?"}}</c></description>
    /// </item>
    /// </list>
    /// The listens are comma-separated, so we need to add one to the listen size and subtract one from the fixed overhead.<br/>
    /// So the maximum listens that can be submitted at once is ((<see cref="MaxListenSize"/> - 36) / 72) (currently 141).
    /// </remarks>
    private const int MaxListensInOnePayload = ((ListenBrainz.MaxListenSize - 36) / 72);

    /// <summary>Imports a set of listens for the user whose token is set in <see cref="UserToken"/>.</summary>
    /// <param name="listens">The listens to import.</param>
    /// <remarks>
    /// This will access the <c>POST /1/submit-listens</c> endpoint.<br/>
    /// Users can find their token on their profile page:
    /// <a href="https://listenbrainz.org/profile/">https://listenbrainz.org/profile/</a>.<br/>
    /// Submissions will happen every <see cref="MaxListensInOnePayload"/> listens, and if a submission's listen data would exceed
    /// <see cref="MaxListenSize"/>, this will split them up and submit them in chunks to avoid hitting that limit. As such, one
    /// call to this method may result in multiple web service requests, which may affect rate limiting.
    /// </remarks>
    public void ImportListens(IEnumerable<ISubmittedListen> listens)
      => ListenBrainz.ResultOf(this.ImportListensAsync(listens));

    /// <summary>Imports a set of listens for the user whose token is set in <see cref="UserToken"/>.</summary>
    /// <param name="listens">The listens to import.</param>
    /// <remarks>
    /// This will access the <c>POST /1/submit-listens</c> endpoint.<br/>
    /// Users can find their token on their profile page:
    /// <a href="https://listenbrainz.org/profile/">https://listenbrainz.org/profile/</a>.<br/>
    /// Submissions will happen every <see cref="MaxListensInOnePayload"/> listens, and if a submission's listen data would exceed
    /// <see cref="MaxListenSize"/>, this will split them up and submit them in chunks to avoid hitting that limit. As such, one
    /// call to this method may result in multiple web service requests, which may affect rate limiting.
    /// </remarks>
    public void ImportListens(params ISubmittedListen[] listens)
      => ListenBrainz.ResultOf(this.ImportListensAsync(listens));

    /// <summary>Imports a set of listens for the user whose token is set in <see cref="UserToken"/>.</summary>
    /// <param name="listens">The listens to import.</param>
    /// <return>A task that will perform the operation.</return>
    /// <remarks>
    /// This will access the <c>POST /1/submit-listens</c> endpoint.<br/>
    /// Users can find their token on their profile page:
    /// <a href="https://listenbrainz.org/profile/">https://listenbrainz.org/profile/</a>.<br/>
    /// Submissions will happen every <see cref="MaxListensInOnePayload"/> listens, and if a submission's listen data would exceed
    /// <see cref="MaxListenSize"/>, this will split them up and submit them in chunks to avoid hitting that limit. As such, one
    /// call to this method may result in multiple web service requests, which may affect rate limiting.
    /// </remarks>
    public async Task ImportListensAsync(IAsyncEnumerable<ISubmittedListen> listens) {
      var payload = SubmissionPayload.CreateImport();
      await foreach(var listen in listens) {
        payload.Listens.Add(listen);
        if (payload.Listens.Count < ListenBrainz.MaxListensInOnePayload)
          continue;
        await this.ImportListensAsync(this.SerializeImport(payload)).ConfigureAwait(false);
        payload.Listens.Clear();
      }
      if (payload.Listens.Count == 0)
        return;
      await this.ImportListensAsync(this.SerializeImport(payload)).ConfigureAwait(false);
    }

    /// <summary>Imports a set of listens for the user whose token is set in <see cref="UserToken"/>.</summary>
    /// <param name="listens">The listens to import.</param>
    /// <return>A task that will perform the operation.</return>
    /// <remarks>
    /// This will access the <c>POST /1/submit-listens</c> endpoint.<br/>
    /// Users can find their token on their profile page:
    /// <a href="https://listenbrainz.org/profile/">https://listenbrainz.org/profile/</a>.<br/>
    /// Submissions will happen every <see cref="MaxListensInOnePayload"/> listens, and if a submission's listen data would exceed
    /// <see cref="MaxListenSize"/>, this will split them up and submit them in chunks to avoid hitting that limit. As such, one
    /// call to this method may result in multiple web service requests, which may affect rate limiting.
    /// </remarks>
    public async Task ImportListensAsync(IEnumerable<ISubmittedListen> listens) {
      var payload = SubmissionPayload.CreateImport();
      foreach(var listen in listens) {
        payload.Listens.Add(listen);
        if (payload.Listens.Count < ListenBrainz.MaxListensInOnePayload)
          continue;
        await this.ImportListensAsync(this.SerializeImport(payload)).ConfigureAwait(false);
        payload.Listens.Clear();
      }
      if (payload.Listens.Count == 0)
        return;
      await this.ImportListensAsync(this.SerializeImport(payload)).ConfigureAwait(false);
    }

    private async Task ImportListensAsync(IEnumerable<string> serializedListens) {
      foreach (var json in serializedListens)
        await this.SubmitListensAsync(json);
    }

    /// <summary>Imports a set of listens for the user whose token is set in <see cref="UserToken"/>.</summary>
    /// <param name="listens">The listens to import.</param>
    /// <return>A task that will perform the operation.</return>
    /// <remarks>
    /// This will access the <c>POST /1/submit-listens</c> endpoint.<br/>
    /// Users can find their token on their profile page:
    /// <a href="https://listenbrainz.org/profile/">https://listenbrainz.org/profile/</a>.<br/>
    /// Submissions will happen every <see cref="MaxListensInOnePayload"/> listens, and if a submission's listen data would exceed
    /// <see cref="MaxListenSize"/>, this will split them up and submit them in chunks to avoid hitting that limit. As such, one
    /// call to this method may result in multiple web service requests, which may affect rate limiting.
    /// </remarks>
    public async Task ImportListensAsync(params ISubmittedListen[] listens) {
      var payload = SubmissionPayload.CreateImport();
      foreach(var listen in listens) {
        payload.Listens.Add(listen);
        if (payload.Listens.Count < ListenBrainz.MaxListensInOnePayload)
          continue;
        await this.ImportListensAsync(this.SerializeImport(payload)).ConfigureAwait(false);
        payload.Listens.Clear();
      }
      if (payload.Listens.Count == 0)
        return;
      await this.ImportListensAsync(this.SerializeImport(payload)).ConfigureAwait(false);
    }

    private IEnumerable<string> SerializeImport(SubmissionPayload<ISubmittedListen> payload) {
      var json = JsonSerializer.Serialize(payload, ListenBrainz.JsonWriterOptions);
      // If it's small enough, or we can't split up the listens, we're done
      if (json.Length <= ListenBrainz.MaxListenSize || payload.Listens.Count <= 1) {
        yield return json;
        yield break;
      }
      // Otherwise, split the list of listens in half
      var firstHalf = payload.Listens.Count / 2;
      var secondHalf = payload.Listens.Count - firstHalf;
      { // Recurse over first half
        var partialPayload = SubmissionPayload.CreateImport();
        partialPayload.Listens.AddRange(payload.Listens.GetRange(0, firstHalf));
        foreach (var part in this.SerializeImport(partialPayload))
          yield return part;
      }
      { // Recurse over second half
        var partialPayload = SubmissionPayload.CreateImport();
        partialPayload.Listens.AddRange(payload.Listens.GetRange(firstHalf, secondHalf));
        foreach (var part in this.SerializeImport(partialPayload))
          yield return part;
      }
    }

    #endregion

    #region Set "Now Playing"

    /// <summary>Sets the "now playing" information for the user whose token is set in <see cref="UserToken"/>.</summary>
    /// <param name="listen">The listen data to send.</param>
    /// <remarks>
    /// This will access the <c>POST /1/submit-listens</c> endpoint.<br/>
    /// Users can find their token on their profile page:
    /// <a href="https://listenbrainz.org/profile/">https://listenbrainz.org/profile/</a>.
    /// </remarks>
    public void SetNowPlaying(ISubmittedListenData listen)
      => ListenBrainz.ResultOf(this.SetNowPlayingAsync(listen));

    /// <summary>Sets the "now playing" information for the user whose token is set in <see cref="UserToken"/>.</summary>
    /// <param name="track">The name of the track being listened to.</param>
    /// <param name="artist">The name of the artist performing the track being listened to.</param>
    /// <param name="release">The name of the release containing the track being listened to.</param>
    /// <remarks>
    /// This will access the <c>POST /1/submit-listens</c> endpoint.<br/>
    /// Users can find their token on their profile page:
    /// <a href="https://listenbrainz.org/profile/">https://listenbrainz.org/profile/</a>.
    /// </remarks>
    public void SetNowPlaying(string track, string artist, string? release = null)
      => ListenBrainz.ResultOf(this.SetNowPlayingAsync(track, artist, release));

    /// <summary>Sets the "now playing" information for the user whose token is set in <see cref="UserToken"/>.</summary>
    /// <param name="listen">The listen data to send.</param>
    /// <return>A task that will perform the operation.</return>
    /// <remarks>
    /// This will access the <c>POST /1/submit-listens</c> endpoint.<br/>
    /// Users can find their token on their profile page:
    /// <a href="https://listenbrainz.org/profile/">https://listenbrainz.org/profile/</a>.
    /// </remarks>
    public async Task SetNowPlayingAsync(ISubmittedListenData listen)
      => await this.SubmitListensAsync(SubmissionPayload.CreatePlayingNow(listen));

    /// <summary>Sets the "now playing" information for the user whose token is set in <see cref="UserToken"/>.</summary>
    /// <param name="track">The name of the track being listened to.</param>
    /// <param name="artist">The name of the artist performing the track being listened to.</param>
    /// <param name="release">The name of the release containing the track being listened to.</param>
    /// <return>A task that will perform the operation.</return>
    /// <remarks>
    /// This will access the <c>POST /1/submit-listens</c> endpoint.<br/>
    /// Users can find their token on their profile page:
    /// <a href="https://listenbrainz.org/profile/">https://listenbrainz.org/profile/</a>.
    /// </remarks>
    public Task SetNowPlayingAsync(string track, string artist, string? release = null)
      => this.SetNowPlayingAsync(new SubmittedListenData(track, artist, release));

    #endregion

    #region Submit Single Listen

    /// <summary>
    /// Submits a single listen (typically one that has just completed) for the user whose token is set in <see cref="UserToken"/>.
    /// </summary>
    /// <param name="listen">The listen to send.</param>
    /// <remarks>
    /// This will access the <c>POST /1/submit-listens</c> endpoint.<br/>
    /// Users can find their token on their profile page:
    /// <a href="https://listenbrainz.org/profile/">https://listenbrainz.org/profile/</a>.
    /// </remarks>
    public void SubmitSingleListen(ISubmittedListen listen)
      => ListenBrainz.ResultOf(this.SubmitSingleListenAsync(listen));

    /// <summary>
    /// Submits a single listen (typically one that has just completed) for the user whose token is set in <see cref="UserToken"/>.
    /// </summary>
    /// <param name="timestamp">The date and time at which the track was listened to.</param>
    /// <param name="track">The name of the track being listened to.</param>
    /// <param name="artist">The name of the artist performing the track being listened to.</param>
    /// <param name="release">The name of the release containing the track being listened to.</param>
    /// <remarks>
    /// This will access the <c>POST /1/submit-listens</c> endpoint.<br/>
    /// Users can find their token on their profile page:
    /// <a href="https://listenbrainz.org/profile/">https://listenbrainz.org/profile/</a>.
    /// </remarks>
    public void SubmitSingleListen(DateTimeOffset timestamp, string track, string artist, string? release = null)
      => ListenBrainz.ResultOf(this.SubmitSingleListenAsync(timestamp, track, artist, release));

    /// <summary>
    /// Submits a single listen (typically one that has just completed) for the user whose token is set in <see cref="UserToken"/>.
    /// </summary>
    /// <param name="timestamp">
    /// The date and time at which the track was listened to, expressed as the number of seconds since
    /// <see cref="UnixTime.Epoch">the Unix time epoch</see>.
    /// </param>
    /// <param name="track">The name of the track being listened to.</param>
    /// <param name="artist">The name of the artist performing the track being listened to.</param>
    /// <param name="release">The name of the release containing the track being listened to.</param>
    /// <remarks>
    /// This will access the <c>POST /1/submit-listens</c> endpoint.<br/>
    /// Users can find their token on their profile page:
    /// <a href="https://listenbrainz.org/profile/">https://listenbrainz.org/profile/</a>.
    /// </remarks>
    public void SubmitSingleListen(long timestamp, string track, string artist, string? release = null)
      => ListenBrainz.ResultOf(this.SubmitSingleListenAsync(timestamp, track, artist, release));

    /// <summary>
    /// Submits a single listen for the user whose token is set in <see cref="UserToken"/>, using the current (UTC) date and time as
    /// timestamp.
    /// </summary>
    /// <param name="track">The name of the track being listened to.</param>
    /// <param name="artist">The name of the artist performing the track being listened to.</param>
    /// <param name="release">The name of the release containing the track being listened to.</param>
    /// <remarks>
    /// This will access the <c>POST /1/submit-listens</c> endpoint.<br/>
    /// Users can find their token on their profile page:
    /// <a href="https://listenbrainz.org/profile/">https://listenbrainz.org/profile/</a>.
    /// </remarks>
    public void SubmitSingleListen(string track, string artist, string? release = null)
      => ListenBrainz.ResultOf(this.SubmitSingleListenAsync(track, artist, release));

    /// <summary>
    /// Submits a single listen (typically one that has just completed) for the user whose token is set in <see cref="UserToken"/>.
    /// </summary>
    /// <param name="listen">The listen data to send.</param>
    /// <return>A task that will perform the operation.</return>
    /// <remarks>
    /// This will access the <c>POST /1/submit-listens</c> endpoint.<br/>
    /// Users can find their token on their profile page:
    /// <a href="https://listenbrainz.org/profile/">https://listenbrainz.org/profile/</a>.
    /// </remarks>
    public async Task SubmitSingleListenAsync(ISubmittedListen listen)
      => await this.SubmitListensAsync(SubmissionPayload.CreateSingle(listen));

    /// <summary>
    /// Submits a single listen (typically one that has just completed) for the user whose token is set in <see cref="UserToken"/>.
    /// </summary>
    /// <param name="timestamp">The date and time at which the track was listened to.</param>
    /// <param name="track">The name of the track being listened to.</param>
    /// <param name="artist">The name of the artist performing the track being listened to.</param>
    /// <param name="release">The name of the release containing the track being listened to.</param>
    /// <return>A task that will perform the operation.</return>
    /// <remarks>
    /// This will access the <c>POST /1/submit-listens</c> endpoint.<br/>
    /// Users can find their token on their profile page:
    /// <a href="https://listenbrainz.org/profile/">https://listenbrainz.org/profile/</a>.
    /// </remarks>
    public Task SubmitSingleListenAsync(DateTimeOffset timestamp, string track, string artist, string? release = null)
      => this.SubmitSingleListenAsync(new SubmittedListen(timestamp, track, artist, release));

    /// <summary>
    /// Submits a single listen (typically one that has just completed) for the user whose token is set in <see cref="UserToken"/>.
    /// </summary>
    /// <param name="timestamp">
    /// The date and time at which the track was listened to, expressed as the number of seconds since
    /// <see cref="UnixTime.Epoch">the Unix time epoch</see>.
    /// </param>
    /// <param name="track">The name of the track being listened to.</param>
    /// <param name="artist">The name of the artist performing the track being listened to.</param>
    /// <param name="release">The name of the release containing the track being listened to.</param>
    /// <return>A task that will perform the operation.</return>
    /// <remarks>
    /// This will access the <c>POST /1/submit-listens</c> endpoint.<br/>
    /// Users can find their token on their profile page:
    /// <a href="https://listenbrainz.org/profile/">https://listenbrainz.org/profile/</a>.
    /// </remarks>
    public Task SubmitSingleListenAsync(long timestamp, string track, string artist, string? release = null)
      => this.SubmitSingleListenAsync(new SubmittedListen(timestamp, track, artist, release));

    /// <summary>
    /// Submits a single listen for the user whose token is set in <see cref="UserToken"/>, using the current (UTC) date and time as
    /// timestamp.
    /// </summary>
    /// <param name="track">The name of the track being listened to.</param>
    /// <param name="artist">The name of the artist performing the track being listened to.</param>
    /// <param name="release">The name of the release containing the track being listened to.</param>
    /// <remarks>
    /// This will access the <c>POST /1/submit-listens</c> endpoint.<br/>
    /// Users can find their token on their profile page:
    /// <a href="https://listenbrainz.org/profile/">https://listenbrainz.org/profile/</a>.
    /// </remarks>
    public Task SubmitSingleListenAsync(string track, string artist, string? release = null)
      => this.SubmitSingleListenAsync(new SubmittedListen(track, artist, release));

    #endregion

    #endregion

    #region /1/user/xxx/listen-count

    /// <summary>Gets the number of listens submitted to ListenBrainz by a particular user.</summary>
    /// <param name="user">The MusicBrainz ID of the user whose listen count is requested.</param>
    /// <returns>An object providing the number of listens submitted by <paramref name="user"/>.</returns>
    /// <remarks>This will access the <c>GET /1/user/USER/listen-count</c> endpoint.</remarks>
    public IListenCount GetListenCount(string user)
      => ListenBrainz.ResultOf(this.GetListenCountAsync(user));

    /// <summary>Gets the number of listens submitted to ListenBrainz by a particular user.</summary>
    /// <param name="user">The MusicBrainz ID of the user whose listen count is requested.</param>
    /// <returns>An object providing the number of listens submitted by <paramref name="user"/>.</returns>
    /// <remarks>This will access the <c>GET /1/user/USER/listen-count</c> endpoint.</remarks>
    public async Task<IListenCount> GetListenCountAsync(string user)
      => await this.GetAsync<IListenCount, ListenCount>($"user/{user}/listen-count");

    #endregion

    #region /1/user/xxx/listens

    #region Internal Helpers

    private static IDictionary<string, string> OptionsForGetListens(int? count, long? after, long? before, int? timeRange) {
      var options = new Dictionary<string, string>(3);
      if (count.HasValue)
        options.Add("count", count.Value.ToString(CultureInfo.InvariantCulture));
      if (before.HasValue)
        options.Add("max_ts", before.Value.ToString(CultureInfo.InvariantCulture));
      if (after.HasValue)
        options.Add("min_ts", after.Value.ToString(CultureInfo.InvariantCulture));
      if (timeRange.HasValue)
        options.Add("time_range", timeRange.Value.ToString(CultureInfo.InvariantCulture));
      return options;
    }

    private IFetchedListens PerformGetListens(string user, long? after, long? before, int? count = null, int? timeRange = null)
      => ListenBrainz.ResultOf(this.PerformGetListensAsync(user, after, before, count, timeRange));

    private Task<IFetchedListens> PerformGetListensAsync(string user, long? after, long? before, int? count = null, int? timeRange = null) {
      var options = ListenBrainz.OptionsForGetListens(count, after, before, timeRange);
      return this.GetAsync<IFetchedListens, FetchedListens>($"user/{user}/listens", options);
    }

    #endregion

    #region No Timestamps

    /// <summary>Gets the most recent listens for a user.</summary>
    /// <param name="user">The MusicBrainz ID of the user whose data is needed.</param>
    /// <param name="count">
    /// The (maximum) number of listens to return; must be no greater than <see cref="MaxItemsPerGet"/>.<br/>
    /// If not specified, this will return up to <see cref="DefaultItemsPerGet"/> listens.
    /// </param>
    /// <param name="timeRange">
    /// The time range to search, in sets of 5 days; must be no greater than <see cref="MaxTimeRange"/>.<br/>
    /// If not specified, <see cref="DefaultTimeRange"/> will be used as time range.
    /// </param>
    /// <returns>The requested listens, in descending timestamp order.</returns>
    public IFetchedListens GetListens(string user, int? count = null, int? timeRange = null)
      => this.PerformGetListens(user, null, null, count, timeRange);

    /// <summary>Gets the most recent listens for a user.</summary>
    /// <param name="user">The MusicBrainz ID of the user whose data is needed.</param>
    /// <param name="count">
    /// The (maximum) number of listens to return; must be no greater than <see cref="MaxItemsPerGet"/>.<br/>
    /// If not specified, this will return up to <see cref="DefaultItemsPerGet"/> listens.
    /// </param>
    /// <param name="timeRange">
    /// The time range to search, in sets of 5 days; must be no greater than <see cref="MaxTimeRange"/>.<br/>
    /// If not specified, <see cref="DefaultTimeRange"/> will be used as time range.
    /// </param>
    /// <returns>The requested listens, in descending timestamp order.</returns>
    public async Task<IFetchedListens> GetListensAsync(string user, int? count = null, int? timeRange = null)
      => await this.PerformGetListensAsync(user, null, null, count, timeRange);

    #endregion

    #region Minimum Timestamp

    /// <summary>Gets the most recent listens for a user, starting from a particular timestamp.</summary>
    /// <param name="user">The MusicBrainz ID of the user whose data is needed.</param>
    /// <param name="after">
    /// The timestamp to start from, expressed as the number of seconds since <see cref="UnixTime.Epoch">the Unix time epoch</see>.
    /// Returned listens will have a timestamp greater than, but not including, this value.
    /// </param>
    /// <param name="count">
    /// The (maximum) number of listens to return; must be no greater than <see cref="MaxItemsPerGet"/>.<br/>
    /// If not specified, this will return up to <see cref="DefaultItemsPerGet"/> listens.
    /// </param>
    /// <param name="timeRange">
    /// The time range to search, in sets of 5 days; must be no greater than <see cref="MaxTimeRange"/>.<br/>
    /// If not specified, <see cref="DefaultTimeRange"/> will be used as time range.
    /// </param>
    /// <returns>The requested listens, in descending timestamp order.</returns>
    public IFetchedListens GetListensAfter(string user, long after, int? count = null, int? timeRange = null)
      => this.PerformGetListens(user, after, null, count, timeRange);

    /// <summary>Gets the most recent listens for a user, starting from a particular timestamp.</summary>
    /// <param name="user">The MusicBrainz ID of the user whose data is needed.</param>
    /// <param name="after">
    /// The timestamp to start from (with a precision of seconds).
    /// Returned listens will have a timestamp greater than, but not including, this value.
    /// </param>
    /// <param name="count">
    /// The (maximum) number of listens to return; must be no greater than <see cref="MaxItemsPerGet"/>.<br/>
    /// If not specified, this will return up to <see cref="DefaultItemsPerGet"/> listens.
    /// </param>
    /// <param name="timeRange">
    /// The time range to search, in sets of 5 days; must be no greater than <see cref="MaxTimeRange"/>.<br/>
    /// If not specified, <see cref="DefaultTimeRange"/> will be used as time range.
    /// </param>
    /// <returns>The requested listens, in descending timestamp order.</returns>
    public IFetchedListens GetListensAfter(string user, DateTimeOffset after, int? count = null, int? timeRange = null)
      => this.PerformGetListens(user, UnixTime.Convert(after), null, count, timeRange);

    /// <summary>Gets the most recent listens for a user, starting from a particular timestamp.</summary>
    /// <param name="user">The MusicBrainz ID of the user whose data is needed.</param>
    /// <param name="after">
    /// The timestamp to start from, expressed as the number of seconds since <see cref="UnixTime.Epoch">the Unix time epoch</see>.
    /// Returned listens will have a timestamp greater than, but not including, this value.
    /// </param>
    /// <param name="count">
    /// The (maximum) number of listens to return; must be no greater than <see cref="MaxItemsPerGet"/>.<br/>
    /// If not specified, this will return up to <see cref="DefaultItemsPerGet"/> listens.
    /// </param>
    /// <param name="timeRange">
    /// The time range to search, in sets of 5 days; must be no greater than <see cref="MaxTimeRange"/>.<br/>
    /// If not specified, <see cref="DefaultTimeRange"/> will be used as time range.
    /// </param>
    /// <returns>The requested listens, in descending timestamp order.</returns>
    public async Task<IFetchedListens> GetListensAfterAsync(string user, long after, int? count = null, int? timeRange = null)
      => await this.PerformGetListensAsync(user, after, null, count, timeRange);

    /// <summary>Gets the most recent listens for a user, starting from a particular timestamp.</summary>
    /// <param name="user">The MusicBrainz ID of the user whose data is needed.</param>
    /// <param name="after">
    /// The timestamp to start from (with a precision of seconds).
    /// Returned listens will have a timestamp greater than, but not including, this value.
    /// </param>
    /// <param name="count">
    /// The (maximum) number of listens to return; must be no greater than <see cref="MaxItemsPerGet"/>.<br/>
    /// If not specified, this will return up to <see cref="DefaultItemsPerGet"/> listens.
    /// </param>
    /// <param name="timeRange">
    /// The time range to search, in sets of 5 days; must be no greater than <see cref="MaxTimeRange"/>.<br/>
    /// If not specified, <see cref="DefaultTimeRange"/> will be used as time range.
    /// </param>
    /// <returns>The requested listens, in descending timestamp order.</returns>
    public async Task<IFetchedListens> GetListensAfterAsync(string user, DateTimeOffset after, int? count = null, int? timeRange = null)
      => await this.PerformGetListensAsync(user, UnixTime.Convert(after), null, count, timeRange);

    #endregion

    #region Maximum Timestamp

    /// <summary>Gets historical listens for a user, starting from a particular timestamp.</summary>
    /// <param name="user">The MusicBrainz ID of the user whose data is needed.</param>
    /// <param name="before">
    /// The timestamp to start from, expressed as the number of seconds since <see cref="UnixTime.Epoch">the Unix time epoch</see>.
    /// Returned listens will have a timestamp less than, but not including, this value.
    /// </param>
    /// <param name="count">
    /// The (maximum) number of listens to return; must be no greater than <see cref="MaxItemsPerGet"/>.<br/>
    /// If not specified, this will return up to <see cref="DefaultItemsPerGet"/> listens.
    /// </param>
    /// <param name="timeRange">
    /// The time range to search, in sets of 5 days; must be no greater than <see cref="MaxTimeRange"/>.<br/>
    /// If not specified, <see cref="DefaultTimeRange"/> will be used as time range.
    /// </param>
    /// <returns>The requested listens, in descending timestamp order.</returns>
    public IFetchedListens GetListensBefore(string user, long before, int? count = null, int? timeRange = null)
      => this.PerformGetListens(user, null, before, count, timeRange);

    /// <summary>Gets historical listens for a user, starting from a particular timestamp.</summary>
    /// <param name="user">The MusicBrainz ID of the user whose data is needed.</param>
    /// <param name="before">
    /// The timestamp to start from (with a precision of seconds).
    /// Returned listens will have a timestamp less than, but not including, this value.
    /// </param>
    /// <param name="count">
    /// The (maximum) number of listens to return; must be no greater than <see cref="MaxItemsPerGet"/>.<br/>
    /// If not specified, this will return up to <see cref="DefaultItemsPerGet"/> listens.
    /// </param>
    /// <param name="timeRange">
    /// The time range to search, in sets of 5 days; must be no greater than <see cref="MaxTimeRange"/>.<br/>
    /// If not specified, <see cref="DefaultTimeRange"/> will be used as time range.
    /// </param>
    /// <returns>The requested listens, in descending timestamp order.</returns>
    public IFetchedListens GetListensBefore(string user, DateTimeOffset before, int? count = null, int? timeRange = null)
      => this.PerformGetListens(user, null, UnixTime.Convert(before), count, timeRange);

    /// <summary>Gets historical listens for a user, starting from a particular timestamp.</summary>
    /// <param name="user">The MusicBrainz ID of the user whose data is needed.</param>
    /// <param name="before">
    /// The timestamp to start from, expressed as the number of seconds since <see cref="UnixTime.Epoch">the Unix time epoch</see>.
    /// Returned listens will have a timestamp less than, but not including, this value.
    /// </param>
    /// <param name="count">
    /// The (maximum) number of listens to return; must be no greater than <see cref="MaxItemsPerGet"/>.<br/>
    /// If not specified, this will return up to <see cref="DefaultItemsPerGet"/> listens.
    /// </param>
    /// <param name="timeRange">
    /// The time range to search, in sets of 5 days; must be no greater than <see cref="MaxTimeRange"/>.<br/>
    /// If not specified, <see cref="DefaultTimeRange"/> will be used as time range.
    /// </param>
    /// <returns>The requested listens, in descending timestamp order.</returns>
    public async Task<IFetchedListens> GetListensBeforeAsync(string user, long before, int? count = null, int? timeRange = null)
      => await this.PerformGetListensAsync(user, null, before, count, timeRange);

    /// <summary>Gets historical listens for a user, starting from a particular timestamp.</summary>
    /// <param name="user">The MusicBrainz ID of the user whose data is needed.</param>
    /// <param name="before">
    /// The timestamp to start from (with a precision of seconds).
    /// Returned listens will have a timestamp less than, but not including, this value.
    /// </param>
    /// <param name="count">
    /// The (maximum) number of listens to return; must be no greater than <see cref="MaxItemsPerGet"/>.<br/>
    /// If not specified, this will return up to <see cref="DefaultItemsPerGet"/> listens.
    /// </param>
    /// <param name="timeRange">
    /// The time range to search, in sets of 5 days; must be no greater than <see cref="MaxTimeRange"/>.<br/>
    /// If not specified, <see cref="DefaultTimeRange"/> will be used as time range.
    /// </param>
    /// <returns>The requested listens, in descending timestamp order.</returns>
    public async Task<IFetchedListens> GetListensBeforeAsync(string user, DateTimeOffset before, int? count = null, int? timeRange = null)
      => await this.PerformGetListensAsync(user, null, UnixTime.Convert(before), count, timeRange);

    #endregion

    #region Both Timestamps

    /// <summary>Gets the listens for a user in a specific timespan.</summary>
    /// <param name="user">The MusicBrainz ID of the user whose data is needed.</param>
    /// <param name="after">
    /// The timestamp to start from, expressed as the number of seconds since <see cref="UnixTime.Epoch">the Unix time epoch</see>.
    /// Returned listens will have a timestamp greater than, but not including, this value.
    /// </param>
    /// <param name="before">
    /// The timestamp to end at, expressed as the number of seconds since <see cref="UnixTime.Epoch">the Unix time epoch</see>.
    /// Returned listens will have a timestamp less than, but not including, this value.
    /// </param>
    /// <param name="count">
    /// The (maximum) number of listens to return; must be no greater than <see cref="MaxItemsPerGet"/>.<br/>
    /// If not specified, this will return up to <see cref="DefaultItemsPerGet"/> listens.
    /// </param>
    /// <returns>The requested listens, in descending timestamp order.</returns>
    public IFetchedListens GetListensBetween(string user, long after, long before, int? count = null)
      => this.PerformGetListens(user, after, before, count);

    /// <summary>Gets the listens for a user in a specific timespan.</summary>
    /// <param name="user">The MusicBrainz ID of the user whose data is needed.</param>
    /// <param name="after">
    /// The timestamp to end at (with a precision of seconds).
    /// Returned listens will have a timestamp greater than, but not including, this value.
    /// </param>
    /// <param name="before">
    /// The timestamp to start from (with a precision of seconds).
    /// Returned listens will have a timestamp less than, but not including, this value.
    /// </param>
    /// <param name="count">
    /// The (maximum) number of listens to return; must be no greater than <see cref="MaxItemsPerGet"/>.<br/>
    /// If not specified, this will return up to <see cref="DefaultItemsPerGet"/> listens.
    /// </param>
    /// <returns>The requested listens, in descending timestamp order.</returns>
    public IFetchedListens GetListensBetween(string user, DateTimeOffset after, DateTimeOffset before, int? count = null)
      => this.PerformGetListens(user, UnixTime.Convert(after), UnixTime.Convert(before), count);

    /// <summary>Gets the listens for a user in a specific timespan.</summary>
    /// <param name="user">The MusicBrainz ID of the user whose data is needed.</param>
    /// <param name="after">
    /// The timestamp to start from, expressed as the number of seconds since <see cref="UnixTime.Epoch">the Unix time epoch</see>.
    /// Returned listens will have a timestamp greater than, but not including, this value.
    /// </param>
    /// <param name="before">
    /// The timestamp to end at, expressed as the number of seconds since <see cref="UnixTime.Epoch">the Unix time epoch</see>.
    /// Returned listens will have a timestamp less than, but not including, this value.
    /// </param>
    /// <param name="count">
    /// The (maximum) number of listens to return; must be no greater than <see cref="MaxItemsPerGet"/>.<br/>
    /// If not specified, this will return up to <see cref="DefaultItemsPerGet"/> listens.
    /// </param>
    /// <returns>The requested listens, in descending timestamp order.</returns>
    public async Task<IFetchedListens> GetListensBetweenAsync(string user, long after, long before, int? count = null)
      => await this.PerformGetListensAsync(user, after, before, count);

    /// <summary>Gets the listens for a user in a specific timespan.</summary>
    /// <param name="user">The MusicBrainz ID of the user whose data is needed.</param>
    /// <param name="after">
    /// The timestamp to start from (with a precision of seconds).
    /// Returned listens will have a timestamp greater than, but not including, this value.
    /// </param>
    /// <param name="before">
    /// The timestamp to end at (with a precision of seconds).
    /// Returned listens will have a timestamp less than, but not including, this value.
    /// </param>
    /// <param name="count">
    /// The (maximum) number of listens to return; must be no greater than <see cref="MaxItemsPerGet"/>.<br/>
    /// If not specified, this will return up to <see cref="DefaultItemsPerGet"/> listens.
    /// </param>
    /// <returns>The requested listens, in descending timestamp order.</returns>
    public async Task<IFetchedListens> GetListensBetweenAsync(string user, DateTimeOffset after, DateTimeOffset before, int? count = null)
      => await this.PerformGetListensAsync(user, UnixTime.Convert(after), UnixTime.Convert(before), count);

    #endregion

    #endregion

    #region /1/user/xxx/playing-now

    /// <summary>Gets a user's currently-playing listen(s).</summary>
    /// <param name="user">The MusicBrainz ID of the user whose data is needed.</param>
    /// <returns>The requested listens (typically 0 or 1).</returns>
    public IPlayingNow GetPlayingNow(string user)
      => ListenBrainz.ResultOf(this.GetPlayingNowAsync(user));

    /// <summary>Gets a user's currently-playing listen(s).</summary>
    /// <param name="user">The MusicBrainz ID of the user whose data is needed.</param>
    /// <returns>The requested listens (typically 0 or 1).</returns>
    public Task<IPlayingNow> GetPlayingNowAsync(string user)
      => this.GetAsync<IPlayingNow, PlayingNow>($"user/{user}/playing-now");

    #endregion

    #region /1/users/xxx/recent-listens

    private static string FormatUserList(IEnumerable<string> userList)
      => string.Join(",", userList.Select(Uri.EscapeDataString));

    private static IDictionary<string, string> OptionsForRecentListens(int limit) {
      return new Dictionary<string, string> {
        ["limit"] = limit.ToString(CultureInfo.InvariantCulture)
      };
    }

    /// <summary>Gets recent listen(s) for a set of users.</summary>
    /// <param name="users">The MusicBrainz IDs of the users whose data is needed.</param>
    /// <returns>The requested listens.</returns>
    public IRecentListens GetRecentListens(params string[] users)
      => ListenBrainz.ResultOf(this.GetRecentListensAsync(users));

    /// <summary>Gets recent listen(s) for a set of users.</summary>
    /// <param name="limit">The maximum number of listens to return.</param>
    /// <param name="users">The MusicBrainz IDs of the users whose data is needed.</param>
    /// <returns>The requested listens.</returns>
    public IRecentListens GetRecentListens(int limit, params string[] users)
      => ListenBrainz.ResultOf(this.GetRecentListensAsync(limit, users));

    /// <summary>Gets recent listen(s) for a set of users.</summary>
    /// <param name="users">The MusicBrainz IDs of the users whose data is needed.</param>
    /// <returns>The requested listens.</returns>
    public async Task<IRecentListens> GetRecentListensAsync(params string[] users)
      => await this.GetAsync<IRecentListens, RecentListens>($"users/{ListenBrainz.FormatUserList(users)}/recent-listens");

    /// <summary>Gets recent listen(s) for a set of users.</summary>
    /// <param name="limit">The maximum number of listens to return.</param>
    /// <param name="users">The MusicBrainz IDs of the users whose data is needed.</param>
    /// <returns>The requested listens.</returns>
    public async Task<IRecentListens> GetRecentListensAsync(int limit, params string[] users) {
      var requestUri = $"users/{ListenBrainz.FormatUserList(users)}/recent-listens";
      var options = ListenBrainz.OptionsForRecentListens(limit);
      return await this.GetAsync<IRecentListens, RecentListens>(requestUri, options);
    }

    #endregion

    #region /1/validate-token

    private static Dictionary<string, string> OptionsForTokenValidation(string token)
      => new Dictionary<string, string> {
        ["token"] = token
      };

    /// <summary>Validates a given user token.</summary>
    /// <param name="token">The user token to validate.</param>
    /// <returns>The result of the validation.</returns>
    public ITokenValidationResult ValidateToken(string token)
      => ListenBrainz.ResultOf(this.ValidateTokenAsync(token));

    /// <summary>Validates a given user token.</summary>
    /// <param name="token">The user token to validate.</param>
    /// <returns>The result of the validation.</returns>
    public async Task<ITokenValidationResult> ValidateTokenAsync(string token) {
      var options = ListenBrainz.OptionsForTokenValidation(token);
      return await this.GetAsync<ITokenValidationResult, TokenValidationResult>("validate-token", options);
    }

    #endregion

    #endregion

    #region Internals

    #region JSON Options

    private static readonly JsonSerializerOptions JsonReaderOptions = JsonUtils.CreateReaderOptions(Converters.Readers);

    private static readonly JsonSerializerOptions JsonWriterOptions = JsonUtils.CreateWriterOptions(Converters.Writers);

    #endregion

    #region HTTP Client / IDisposable

    private AuthenticationHeaderValue? Authentication;

    private readonly SemaphoreSlim ClientLock = new SemaphoreSlim(1);

    private bool Disposed;

    private readonly ProductInfoHeaderValue UserAgentContact;

    private readonly ProductInfoHeaderValue UserAgentProduct;

    private HttpClient? TheClient;

    private HttpClient Client {
      get {
        if (this.Disposed)
          throw new ObjectDisposedException(nameof(ListenBrainz));
        if (this.TheClient == null) { // Set up the instance with the invariant settings
          var an = typeof(ListenBrainz).Assembly.GetName();
          this.TheClient = new HttpClient {
            BaseAddress = this.BaseUri,
            DefaultRequestHeaders = {
              Accept = {
                new MediaTypeWithQualityHeaderValue("application/json")
              },
              UserAgent = {
                this.UserAgentProduct,
                this.UserAgentContact,
                new ProductInfoHeaderValue(an.Name, an.Version?.ToString()),
                new ProductInfoHeaderValue($"({ListenBrainz.UserAgentUrl})"),
              },
            }
          };
        }
        return this.TheClient;
      }
    }

    /// <summary>Closes the underlying web service client in use by this ListenBrainz client, if there is one.</summary>
    /// <remarks>The next web service request will create a new client.</remarks>
    public void Close() {
      this.ClientLock.Wait();
      try {
        this.TheClient?.Dispose();
        this.TheClient = null;
      }
      finally {
        this.ClientLock.Release();
      }
    }

    /// <summary>Disposes the web client in use by this ListenBrainz client, if there is one.</summary>
    /// <remarks>Further attempts at web service requests will cause <see cref="ObjectDisposedException"/> to be thrown.</remarks>
    public void Dispose() {
      this.Dispose(true);
      GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing) {
      if (!disposing)
        return;
      try {
        this.Close();
        this.ClientLock.Dispose();
      }
      finally {
        this.Disposed = true;
      }
    }

    /// <summary>Finalizes this instance.</summary>
    ~ListenBrainz() {
      this.Dispose(false);
    }

    #endregion

    #region Basic Request Execution

    private async Task<TInterface> GetAsync<TInterface, TObject>(string address, IDictionary<string, string>? options = null)
      where TInterface : class
      where TObject : class, TInterface
    {
      var response = await this.PerformRequestAsync(address, HttpMethod.Get, null, options);
      // FIXME: Should this use IsSuccessStatusCode? If so, which one(s) should attempt to process the response content?
      if (response.StatusCode != HttpStatusCode.OK)
        throw ListenBrainz.CreateQueryExceptionFor(response);
      return await ListenBrainz.GetJsonContentAsync<TObject>(response);
    }

    private async Task<TInterface?> GetOptionalAsync<TInterface, TObject>(string address, IDictionary<string, string>? options = null)
      where TInterface : class
      where TObject : class, TInterface
    {
      var response = await this.PerformRequestAsync(address, HttpMethod.Get, null, options);
      if (response.StatusCode == HttpStatusCode.NoContent)
        return null;
      // FIXME: Should this use IsSuccessStatusCode? If so, which one(s) should attempt to process the response content?
      if (response.StatusCode != HttpStatusCode.OK)
        throw ListenBrainz.CreateQueryExceptionFor(response);
      return await ListenBrainz.GetJsonContentAsync<TObject>(response);
    }

    private async Task<HttpResponseMessage> PerformRequestAsync(string address, HttpMethod method, string? body, IDictionary<string, string>? options = null) {
      var requestUri = address + ListenBrainz.QueryString(options);
      Debug.Print($"[{DateTime.UtcNow}] WEB SERVICE REQUEST: {method.Method} {this.BaseUri}{requestUri}");
      await this.ClientLock.WaitAsync();
      try {
        var client = this.Client;
        HttpResponseMessage response;
        switch (method.Method) {
          case "GET": {
            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            request.Headers.Authorization = this.Authentication;
            response = await client.SendAsync(request);
            break;
          }
          case "POST": {
            if (body != null)
              Debug.Print($"[{DateTime.UtcNow}] => BODY: {body}");
            var request = new HttpRequestMessage(HttpMethod.Post, requestUri);
            request.Headers.Authorization = this.Authentication;
            request.Content = new StringContent(body ?? "", Encoding.UTF8, "application/json");
            response = await client.SendAsync(request);
            break;
          }
          default:
            throw new QueryException(HttpStatusCode.MethodNotAllowed, $"Unsupported method: {method}");
        }
        Debug.Print($"[{DateTime.UtcNow}] => RESPONSE: {(int) response.StatusCode}/{response.StatusCode} '{response.ReasonPhrase}' (v{response.Version})");
        Debug.Print($"[{DateTime.UtcNow}] => HEADERS: {ListenBrainz.FormatMultiLine(response.Headers.ToString())}");
        Debug.Print($"[{DateTime.UtcNow}] => CONTENT: {response.Content.Headers.ContentType}, {response.Content.Headers.ContentLength ?? 0} byte(s))");
        this.RateLimitInfo = new RateLimitInfo(response.Headers);
        return response;
      }
      finally {
        this.ClientLock.Release();
      }
    }

    private Task PostAsync<T>(string address, T content, IDictionary<string, string>? options = null)
      => this.PostAsync(address, JsonSerializer.Serialize(content, ListenBrainz.JsonWriterOptions), options);

    private async Task PostAsync(string address, string body, IDictionary<string, string>? options = null) {
      var response = await this.PerformRequestAsync(address, HttpMethod.Post, body, options);
      if (!response.IsSuccessStatusCode)
        throw ListenBrainz.CreateQueryExceptionFor(response);
#if DEBUG
      var content = await ListenBrainz.GetStringContentAsync(response);
      Debug.Print($"[{DateTime.UtcNow}] => RESPONSE TEXT: {ListenBrainz.FormatMultiLine(content)}");
#endif
    }

    #endregion

    #region Utility Methods

    private static QueryException CreateQueryExceptionFor(HttpResponseMessage response) {
      string? errorInfo = null;
      if (response.Content.Headers.ContentLength > 0) {
        errorInfo = ListenBrainz.ResultOf(ListenBrainz.GetStringContentAsync(response));
        if (string.IsNullOrWhiteSpace(errorInfo)) {
          Debug.Print($"[{DateTime.UtcNow}] => NO ERROR RESPONSE TEXT");
          errorInfo = null;
        }
        else
          Debug.Print($"[{DateTime.UtcNow}] => ERROR RESPONSE TEXT: {ListenBrainz.FormatMultiLine(errorInfo)}");
      }
      else
        Debug.Print($"[{DateTime.UtcNow}] => NO ERROR RESPONSE CONTENT");
      if (errorInfo != null) {
        try {
          var ei = JsonSerializer.Deserialize<ErrorInfo>(errorInfo, ListenBrainz.JsonReaderOptions);
          errorInfo = ei.Error;
          if (ei.Code != (int) response.StatusCode)
            Debug.Print($"[{DateTime.UtcNow}] => ERROR CODE ({ei.Code}) DOES NOT MATCH HTTP STATUS CODE!");
          if (ei.UnhandledProperties != null) {
            foreach (var prop in ei.UnhandledProperties)
              Debug.Print($"[{DateTime.UtcNow}] => UNEXPECTED ERROR PROPERTY: {prop.Key} -> {prop.Value}");
          }
        }
        catch (Exception e) {
          Debug.Print($"[{DateTime.UtcNow}] => FAILED TO PARSE AS JSON ({e.Message}); USING AS-IS");
        }
      }
      return new QueryException(response.StatusCode, response.ReasonPhrase, errorInfo);
    }

    private static string FormatMultiLine(string text) {
      const string prefix = "<<";
      const string suffix = ">>";
      const string sep = "\n  ";
      char[] newlines = { '\r', '\n' };
      text = text.Replace("\r\n", "\n").TrimEnd(newlines);
      var lines = text.Split(newlines);
      if (lines.Length == 0)
        return prefix + suffix;
      if (lines.Length == 1)
        return prefix + lines[0] + suffix;
      return prefix + sep + string.Join(sep, lines) + "\n" + suffix;
    }

    private static async Task<T> GetJsonContentAsync<T>(HttpResponseMessage response) {
#if NETSTANDARD2_1 || NETCOREAPP3_1 // || NET5_0
      var stream = await response.Content.ReadAsStreamAsync();
      await using var _ = stream.ConfigureAwait(false);
#else
      using var stream = await response.Content.ReadAsStreamAsync();
#endif
      if (stream == null)
        throw new QueryException(HttpStatusCode.NoContent, "Response contained no data.");
      var contentType = response.Content?.Headers?.ContentType;
      // FIXME: Should this check the media type?
      var characterSet = contentType?.CharSet;
      if (string.IsNullOrWhiteSpace(characterSet))
        characterSet = "utf-8";
#if !DEBUG
      if (characterSet == "utf-8") // Directly use the stream
        return await JsonSerializer.DeserializeAsync<T>(stream, ListenBrainz.JsonReaderOptions);
#endif
      var enc = Encoding.GetEncoding(characterSet);
      using var sr = new StreamReader(stream, enc, false, 1024, true);
      var json = await sr.ReadToEndAsync().ConfigureAwait(false);
      Debug.Print($"[{DateTime.UtcNow}] => JSON: {JsonUtils.Prettify(json)}");
      return JsonUtils.Deserialize<T>(json, ListenBrainz.JsonReaderOptions);
    }

    private static async Task<string> GetStringContentAsync(HttpResponseMessage response) {
#if NETSTANDARD2_1 || NETCOREAPP3_1 // || NET5_0
      var stream = await response.Content.ReadAsStreamAsync();
      await using var _ = stream.ConfigureAwait(false);
#else
      using var stream = await response.Content.ReadAsStreamAsync();
#endif
      if (stream == null)
        return "";
      var characterSet = response.Content?.Headers?.ContentEncoding.FirstOrDefault();
      if (string.IsNullOrWhiteSpace(characterSet))
        characterSet = "utf-8";
      var enc = Encoding.GetEncoding(characterSet);
      using var sr = new StreamReader(stream, enc, false, 1024, true);
      return await sr.ReadToEndAsync().ConfigureAwait(false);
    }

    private static string QueryString(IDictionary<string, string>? options) {
      if (options == null || options.Count == 0)
        return "";
      var sb = new StringBuilder();
      var separator = '?';
      foreach (var option in options) {
        // FIXME: Which parts (if any) need URL/Data escaping?
        sb.Append(separator).Append(option.Key).Append('=').Append(option.Value);
        separator = '&';
      }
      return sb.ToString();
    }

    private static void ResultOf(Task task) => task.ConfigureAwait(false).GetAwaiter().GetResult();

    private static T ResultOf<T>(Task<T> task) => task.ConfigureAwait(false).GetAwaiter().GetResult();

    #endregion

    #endregion

  }

}
