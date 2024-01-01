using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using JetBrains.Annotations;

using MetaBrainz.Common;
using MetaBrainz.Common.Json;
using MetaBrainz.ListenBrainz.Interfaces;
using MetaBrainz.ListenBrainz.Json;
using MetaBrainz.ListenBrainz.Objects;

namespace MetaBrainz.ListenBrainz;

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
  public const string UserAgentUrl = "https://github.com/Zastai/MetaBrainz.ListenBrainz";

  /// <summary>The root location of the web service.</summary>
  public const string WebServiceRoot = "/1/";

  #endregion

  #region Static Fields / Properties

  private static int _defaultPort = -1;

  /// <summary>The default port number to use for requests (-1 to not specify any explicit port).</summary>
  public static int DefaultPort {
    get => ListenBrainz._defaultPort;
    set {
      if (value is < -1 or > 65535) {
        throw new ArgumentOutOfRangeException(nameof(ListenBrainz.DefaultPort), value,
                                              "The default port number must not be less than -1 or greater than 65535.");
      }
      ListenBrainz._defaultPort = value;
    }
  }

  private static string _defaultServer = "api.listenbrainz.org";

  /// <summary>The default server to use for requests.</summary>
  public static string DefaultServer {
    get => ListenBrainz._defaultServer;
    set {
      if (string.IsNullOrWhiteSpace(value)) {
        throw new ArgumentException("The default server name must not be blank.", nameof(ListenBrainz.DefaultServer));
      }
      ListenBrainz._defaultServer = value.Trim();
    }
  }

  private static string _defaultUrlScheme = "https";

  /// <summary>The default URL scheme (internet access protocol) to use for requests.</summary>
  public static string DefaultUrlScheme {
    get => ListenBrainz._defaultUrlScheme;
    set {
      if (string.IsNullOrWhiteSpace(value)) {
        throw new ArgumentException("The default URL scheme must not be blank.", nameof(ListenBrainz.DefaultUrlScheme));
      }
      ListenBrainz._defaultUrlScheme = value.Trim();
    }
  }

  /// <summary>The default user agent values to use for requests.</summary>
  public static IList<ProductInfoHeaderValue> DefaultUserAgent { get; } = new List<ProductInfoHeaderValue>();

  /// <summary>The default user token to use for requests; used as initial value for <see cref="UserToken"/>.</summary>
  public static string? DefaultUserToken { get; set; }

  /// <summary>The trace source (named 'MetaBrainz.ListenBrainz') used by this class.</summary>
  public static readonly TraceSource TraceSource = new("MetaBrainz.ListenBrainz", SourceLevels.Off);

  #endregion

  #region Constructors

  /// <summary>
  /// Initializes a new ListenBrainz API client instance.<br/>
  /// An HTTP client will be created when needed and can be discarded again via the <see cref="Close()"/> method.
  /// </summary>
  public ListenBrainz() {
    this.UserToken = ListenBrainz.DefaultUserToken;
    this._clientOwned = true;
  }

  /// <summary>Initializes a new ListenBrainz API client instance using a specific HTTP client.</summary>
  /// <param name="client">The HTTP client to use.</param>
  /// <param name="takeOwnership">
  /// Indicates whether this ListenBrainz API client should take ownership of <paramref name="client"/>.<br/>
  /// If this is <see langword="false"/>, it remains owned by the caller; this means <see cref="Close()"/> will throw an exception
  /// and <see cref="Dispose()"/> will release the reference to <paramref name="client"/> without disposing it.<br/>
  /// If this is <see langword="true"/>, then this object takes ownership and treat it just like an HTTP client it created itself;
  /// this means <see cref="Close()"/> will dispose of it (with further requests creating a new HTTP client) and
  /// <see cref="Dispose()"/> will dispose the HTTP client too. Note that in this case, any default request headers set on
  /// <paramref name="client"/> will <em>not</em> be saved and used for further clients.
  /// </param>
  public ListenBrainz(HttpClient client, bool takeOwnership = false) {
    this.UserToken = ListenBrainz.DefaultUserToken;
    this._client = client;
    this._clientOwned = takeOwnership;
  }

  /// <summary>
  /// Initializes a new ListenBrainz API client instance.<br/>
  /// An HTTP client will be created when needed and can be discarded again via the <see cref="Close()"/> method.
  /// </summary>
  /// <param name="userAgent">The user agent values to use for all requests.</param>
  public ListenBrainz(params ProductInfoHeaderValue[] userAgent) : this() {
    this._userAgent.AddRange(userAgent);
  }

  /// <summary>
  /// Initializes a new ListenBrainz API client instance.<br/>
  /// An HTTP client will be created when needed and can be discarded again via the <see cref="Close()"/> method.
  /// </summary>
  /// <param name="application">The application name to use in the user agent property for all requests.</param>
  /// <param name="version">The version number to use in the user agent property for all requests.</param>
  public ListenBrainz(string application, Version? version) : this(application, version?.ToString()) {
  }

  /// <summary>
  /// Initializes a new ListenBrainz API client instance.<br/>
  /// An HTTP client will be created when needed and can be discarded again via the <see cref="Close()"/> method.
  /// </summary>
  /// <param name="application">The application name to use in the user agent property for all requests.</param>
  /// <param name="version">The version number to use in the user agent property for all requests.</param>
  /// <param name="contact">
  /// The contact address (typically HTTP[S] or MAILTO) to use in the user agent property for all requests.
  /// </param>
  public ListenBrainz(string application, Version? version, Uri contact) : this(application, version?.ToString(), contact.ToString()) {
  }

  /// <summary>
  /// Initializes a new ListenBrainz API client instance.<br/>
  /// An HTTP client will be created when needed and can be discarded again via the <see cref="Close()"/> method.
  /// </summary>
  /// <param name="application">The application name to use in the user agent property for all requests.</param>
  /// <param name="version">The version number to use in the user agent property for all requests.</param>
  /// <param name="contact">
  /// The contact address (typically a URL or email address) to use in the user agent property for all requests.
  /// </param>
  public ListenBrainz(string application, Version? version, string contact) : this(application, version?.ToString(), contact) { }

  /// <summary>
  /// Initializes a new ListenBrainz API client instance.<br/>
  /// An HTTP client will be created when needed and can be discarded again via the <see cref="Close()"/> method.
  /// </summary>
  /// <param name="application">The application name to use in the user agent property for all requests.</param>
  /// <param name="version">The version number to use in the user agent property for all requests.</param>
  public ListenBrainz(string application, string? version) : this() {
    this._userAgent.Add(new ProductInfoHeaderValue(application, version));
  }

  /// <summary>
  /// Initializes a new ListenBrainz API client instance.<br/>
  /// An HTTP client will be created when needed and can be discarded again via the <see cref="Close()"/> method.
  /// </summary>
  /// <param name="application">The application name to use in the user agent property for all requests.</param>
  /// <param name="version">The version number to use in the user agent property for all requests.</param>
  /// <param name="contact">
  /// The contact address (typically HTTP[S] or MAILTO) to use in the user agent property for all requests.
  /// </param>
  public ListenBrainz(string application, string? version, Uri contact) : this(application, version, contact.ToString()) { }

  /// <summary>
  /// Initializes a new ListenBrainz API client instance.<br/>
  /// An HTTP client will be created when needed and can be discarded again via the <see cref="Close()"/> method.
  /// </summary>
  /// <param name="application">The application name to use in the user agent property for all requests.</param>
  /// <param name="version">The version number to use in the user agent property for all requests.</param>
  /// <param name="contact">
  /// The contact address (typically a URL or email address) to use in the user agent property for all requests.
  /// </param>
  public ListenBrainz(string application, string? version, string contact) : this() {
    this._userAgent.Add(new ProductInfoHeaderValue(application, version));
    this._userAgent.Add(new ProductInfoHeaderValue($"({contact})"));
  }

  #endregion

  #region Public Instance Fields / Properties

  /// <summary>The base URI for all requests.</summary>
  public Uri BaseUri => new UriBuilder(this.UrlScheme, this.Server, this.Port, ListenBrainz.WebServiceRoot).Uri;

  private int _port = ListenBrainz.DefaultPort;

  /// <summary>The port number to use for requests (-1 to not specify any explicit port).</summary>
  public int Port {
    get => this._port;
    set {
      if (value is < -1 or > 65535) {
        throw new ArgumentOutOfRangeException(nameof(ListenBrainz.Port), value,
                                              "The port number must not be less than -1 or greater than 65535.");
      }
      this._port = value;
    }
  }

  private RateLimitInfo _rateLimitInfo;

  private readonly ReaderWriterLockSlim _rateLimitLock = new();

  /// <summary>Information about the active rate limiting. Gets refreshed after every API call.</summary>
  public RateLimitInfo RateLimitInfo {
    get {
      this._rateLimitLock.EnterReadLock();
      try {
        return this._rateLimitInfo;
      }
      finally {
        this._rateLimitLock.ExitReadLock();
      }
    }
  }

  private string _server = ListenBrainz.DefaultServer;

  /// <summary>The server to use for requests.</summary>
  public string Server {
    get => this._server;
    set {
      if (string.IsNullOrWhiteSpace(value)) {
        throw new ArgumentException("The server name must not be blank.", nameof(ListenBrainz.Server));
      }
      this._server = value.Trim();
    }
  }

  private string _urlScheme = ListenBrainz.DefaultUrlScheme;

  /// <summary>The URL scheme (internet access protocol) to use for requests.</summary>
  public string UrlScheme {
    get => this._urlScheme;
    set {
      if (string.IsNullOrWhiteSpace(value)) {
        throw new ArgumentException("The URL scheme must not be blank.", nameof(ListenBrainz.UrlScheme));
      }
      this._urlScheme = value.Trim();
    }
  }

  /// <summary>The user agent values to use for requests.</summary>
  /// <remarks>
  /// Note that changes to this list only take effect when a new HTTP client is created. The <see cref="Close()"/> method can be
  /// used to close the current client (if there is one) so that the next request creates a new client.
  /// </remarks>
  public IList<ProductInfoHeaderValue> UserAgent => this._userAgent;

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
    get => this._authorization?.Parameter;
    set => this._authorization = value is null ? null : new AuthenticationHeaderValue("Token", value);
  }

  #endregion

  #region Public API

  #region /1/latest-import

  private static Dictionary<string, string> OptionsForLatestImport(string user) => new() { ["user_name"] = user };

  /// <summary>Get the timestamp of the newest listen submitted by a user in previous imports to ListenBrainz.</summary>
  /// <param name="user">The MusicBrainz ID of the user whose data is requested.</param>
  /// <returns>An object providing the user's ID and latest import timestamp.</returns>
  /// <remarks>This will access the <c>GET /1/latest-import</c> endpoint.</remarks>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public ILatestImport GetLatestImport(string user) => AsyncUtils.ResultOf(this.GetLatestImportAsync(user));

  /// <summary>Get the timestamp of the newest listen submitted by a user in previous imports to ListenBrainz.</summary>
  /// <param name="user">The MusicBrainz ID of the user whose data is requested.</param>
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <returns>An object providing the user's ID and latest import timestamp.</returns>
  /// <remarks>This will access the <c>GET /1/latest-import</c> endpoint.</remarks>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public Task<ILatestImport> GetLatestImportAsync(string user, CancellationToken cancellationToken = default)
    => this.GetAsync<ILatestImport, LatestImport>("latest-import", ListenBrainz.OptionsForLatestImport(user), cancellationToken);

  /// <summary>Set the timestamp of the newest listen submitted by a user in previous imports to ListenBrainz.</summary>
  /// <param name="user">The MusicBrainz ID of the user whose data should be modified.</param>
  /// <param name="timestamp">The timestamp to set.</param>
  /// <remarks>
  /// This will access the <c>POST /1/latest-import</c> endpoint and requires <see cref="UserToken"/> to be set to the token
  /// for <paramref name="user"/>.
  /// </remarks>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public void SetLatestImport(string user, DateTimeOffset timestamp)
    => AsyncUtils.ResultOf(this.SetLatestImportAsync(user, timestamp.ToUnixTimeSeconds()));

  /// <summary>Set the timestamp of the newest listen submitted by a user in previous imports to ListenBrainz.</summary>
  /// <param name="user">The MusicBrainz ID of the user whose data is needed.</param>
  /// <param name="timestamp">
  /// The timestamp to set, expressed as the number of seconds since <see cref="DateTimeOffset.UnixEpoch">the Unix time epoch</see>.
  /// </param>
  /// <remarks>
  /// This will access the <c>POST /1/latest-import</c> endpoint and requires <see cref="UserToken"/> to be set to the token
  /// for <paramref name="user"/>.
  /// </remarks>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public void SetLatestImport(string user, long timestamp) => AsyncUtils.ResultOf(this.SetLatestImportAsync(user, timestamp));

  /// <summary>Set the timestamp of the newest listen submitted by a user in previous imports to ListenBrainz.</summary>
  /// <param name="user">The MusicBrainz ID of the user whose data should be modified.</param>
  /// <param name="timestamp">The timestamp to set.</param>
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <return>A task that will perform the operation.</return>
  /// <remarks>
  /// This will access the <c>POST /1/latest-import</c> endpoint.<br/>
  /// Users can find their token on their profile page:
  /// <a href="https://listenbrainz.org/profile/">https://listenbrainz.org/profile/</a>.
  /// </remarks>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public Task SetLatestImportAsync(string user, DateTimeOffset timestamp, CancellationToken cancellationToken = default)
    => this.SetLatestImportAsync(user, timestamp.ToUnixTimeSeconds(), cancellationToken);

  /// <summary>Set the timestamp of the newest listen submitted by a user in previous imports to ListenBrainz.</summary>
  /// <param name="user">The MusicBrainz ID of the user whose data is needed.</param>
  /// <param name="timestamp">
  /// The timestamp to set, expressed as the number of seconds since <see cref="DateTimeOffset.UnixEpoch">the Unix time epoch</see>.
  /// </param>
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <return>A task that will perform the operation.</return>
  /// <remarks>
  /// This will access the <c>POST /1/latest-import</c> endpoint.<br/>
  /// Users can find their token on their profile page:
  /// <a href="https://listenbrainz.org/profile/">https://listenbrainz.org/profile/</a>.
  /// </remarks>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public Task SetLatestImportAsync(string user, long timestamp, CancellationToken cancellationToken = default)
    => this.PostAsync("latest-import", $"{{ ts: {timestamp} }}", ListenBrainz.OptionsForLatestImport(user), cancellationToken);

  #endregion

  #region /1/stats

  private static IDictionary<string, string> OptionsForGetStatistics(int? count, int? offset, StatisticsRange? range) {
    var options = new Dictionary<string, string>(3);
    if (count is not null) {
      options.Add("count", count.Value.ToString(CultureInfo.InvariantCulture));
    }
    if (offset is not null) {
      options.Add("offset", offset.Value.ToString(CultureInfo.InvariantCulture));
    }
    if (range is not null) {
      options.Add("range", range.Value.ToJson());
    }
    return options;
  }

  #region /1/stats/sitewide

  #region /1/stats/sitewide/artists

  /// <summary>Gets statistics about the most listened-to artists.</summary>
  /// <param name="count">
  /// The (maximum) number of entries to return. If not specified (or <see langword="null"/>), all available information will be
  /// returned.
  /// </param>
  /// <param name="offset">
  /// The offset (from the start of the results) of the statistics to return for each time range. If not specified (or specified
  /// as zero or <see langword="null"/>), the top most listened-to artists will be returned. Note that at most 1000 artists will
  /// be included in the statistics for each time range.
  /// </param>
  /// <param name="range">
  /// The range of data to include in the statistics.<br/>
  /// If this is unspecified or <see cref="StatisticsRange.AllTime"/>, information is returned about all years with at least one
  /// recorded listen. Otherwise, information is returned about both the current and the previous range.
  /// </param>
  /// <returns>The requested artist statistics.</returns>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public ISiteArtistStatistics? GetArtistStatistics(int? count = null, int? offset = null, StatisticsRange? range = null)
    => AsyncUtils.ResultOf(this.GetArtistStatisticsAsync(count, offset, range));

  /// <summary>Gets statistics about the most listened-to artists.</summary>
  /// <param name="count">
  /// The (maximum) number of entries to return. If not specified (or <see langword="null"/>), all available information will be
  /// returned.
  /// </param>
  /// <param name="offset">
  /// The offset (from the start of the results) of the statistics to return for each time range. If not specified (or specified
  /// as zero or <see langword="null"/>), the top most listened-to artists will be returned. Note that at most 1000 artists will
  /// be included in the statistics for each time range.
  /// </param>
  /// <param name="range">
  /// The range of data to include in the statistics.<br/>
  /// If this is unspecified or <see cref="StatisticsRange.AllTime"/>, information is returned about all years with at least one
  /// recorded listen. Otherwise, information is returned about both the current and the previous range.
  /// </param>
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <returns>The requested artist statistics.</returns>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public Task<ISiteArtistStatistics?> GetArtistStatisticsAsync(int? count = null, int? offset = null,
                                                               StatisticsRange? range = null,
                                                               CancellationToken cancellationToken = default) {
    var options = ListenBrainz.OptionsForGetStatistics(count, offset, range);
    return this.GetOptionalAsync<ISiteArtistStatistics, SiteArtistStatistics>("stats/sitewide/artists", options, cancellationToken);
  }

  #endregion

  #endregion

  #region /1/stats/user/xxx

  #region /1/stats/user/xxx/artist-map

  /// <summary>Gets information about the number of artists a user has listened to, grouped by their country.</summary>
  /// <param name="user">The user for whom the information is requested.</param>
  /// <param name="range">The range of data to include in the statistics.</param>
  /// <param name="forceRecalculation">Indicates whether recalculation of the data should be requested.</param>
  /// <returns>The requested information, or <see langword="null"/> if it has not yet been computed for the user.</returns>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public IUserArtistMap? GetArtistMap(string user, StatisticsRange? range = null, bool forceRecalculation = false)
    => AsyncUtils.ResultOf(this.GetArtistMapAsync(user, range, forceRecalculation));

  /// <summary>Gets information about the number of artists a user has listened to, grouped by their country.</summary>
  /// <param name="user">The user for whom the information is requested.</param>
  /// <param name="range">The range of data to include in the statistics.</param>
  /// <param name="forceRecalculation">Indicates whether recalculation of the data should be requested.</param>
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <returns>The requested information, or <see langword="null"/> if it has not yet been computed for the user.</returns>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public Task<IUserArtistMap?> GetArtistMapAsync(string user, StatisticsRange? range = null, bool forceRecalculation = false,
                                                 CancellationToken cancellationToken = default) {
    var options = new Dictionary<string, string>(2);
    if (range is not null) {
      options.Add("range", range.Value.ToJson());
    }
    if (forceRecalculation) {
      options.Add("force_recalculate", "true");
    }
    return this.GetOptionalAsync<IUserArtistMap, UserArtistMap>($"stats/user/{user}/artist-map", options, cancellationToken);
  }

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
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public IUserArtistStatistics? GetArtistStatistics(string user, int? count = null, int? offset = null,
                                                    StatisticsRange? range = null)
    => AsyncUtils.ResultOf(this.GetArtistStatisticsAsync(user, count, offset, range));

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
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <returns>
  /// The requested artist statistics, or <see langword="null"/> if statistics have not yet been computed for the user.
  /// </returns>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public Task<IUserArtistStatistics?> GetArtistStatisticsAsync(string user, int? count = null, int? offset = null,
                                                               StatisticsRange? range = null,
                                                               CancellationToken cancellationToken = default) {
    var address = $"stats/user/{user}/artists";
    var options = ListenBrainz.OptionsForGetStatistics(count, offset, range);
    return this.GetOptionalAsync<IUserArtistStatistics, UserArtistStatistics>(address, options, cancellationToken);
  }

  #endregion

  #region /1/stats/user/xxx/daily-activity

  /// <summary>Gets information about how a user's listens are spread across the days of the week.</summary>
  /// <param name="user">The user for whom the information is requested.</param>
  /// <param name="range">The range of data to include in the information.</param>
  /// <returns>The requested daily activity.</returns>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public IUserDailyActivity? GetDailyActivity(string user, StatisticsRange? range = null)
    => AsyncUtils.ResultOf(this.GetDailyActivityAsync(user, range));

  /// <summary>Gets information about how a user's listens are spread across the days of the week.</summary>
  /// <param name="user">The user for whom the information is requested.</param>
  /// <param name="range">The range of data to include in the information.</param>
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <returns>The requested daily activity.</returns>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public Task<IUserDailyActivity?> GetDailyActivityAsync(string user, StatisticsRange? range = null,
                                                         CancellationToken cancellationToken = default) {
    var address = $"stats/user/{user}/daily-activity";
    var options = ListenBrainz.OptionsForGetStatistics(null, null, range);
    return this.GetOptionalAsync<IUserDailyActivity, UserDailyActivity>(address, options, cancellationToken);
  }

  #endregion

  #region /1/stats/user/xxx/listening-activity

  /// <summary>Gets information about how many listens a user has submitted over a period of time.</summary>
  /// <param name="user">The user for whom the information is requested.</param>
  /// <param name="range">
  /// The range of data to include in the information.<br/>
  /// If this is unspecified or <see cref="StatisticsRange.AllTime"/>, information is returned about all years with at least one
  /// recorded listen. Otherwise, information is returned about both the current and the previous range.
  /// </param>
  /// <returns>The requested listening activity.</returns>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public IUserListeningActivity? GetListeningActivity(string user, StatisticsRange? range = null)
    => AsyncUtils.ResultOf(this.GetListeningActivityAsync(user, range));

  /// <summary>Gets information about how many listens a user has submitted over a period of time.</summary>
  /// <param name="user">The user for whom the information is requested.</param>
  /// <param name="range">
  /// The range of data to include in the information.<br/>
  /// If this is unspecified or <see cref="StatisticsRange.AllTime"/>, information is returned about all years with at least one
  /// recorded listen. Otherwise, information is returned about both the current and the previous range.
  /// </param>
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <returns>The requested listening activity.</returns>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public Task<IUserListeningActivity?> GetListeningActivityAsync(string user, StatisticsRange? range = null,
                                                                 CancellationToken cancellationToken = default) {
    var address = $"stats/user/{user}/listening-activity";
    var options = ListenBrainz.OptionsForGetStatistics(null, null, range);
    return this.GetOptionalAsync<IUserListeningActivity, UserListeningActivity>(address, options, cancellationToken);
  }

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
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public IUserRecordingStatistics? GetRecordingStatistics(string user, int? count = null, int? offset = null,
                                                          StatisticsRange? range = null)
    => AsyncUtils.ResultOf(this.GetRecordingStatisticsAsync(user, count, offset, range));

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
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <returns>
  /// The requested recording statistics, or <see langword="null"/> if statistics have not yet been computed for the user.
  /// </returns>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public Task<IUserRecordingStatistics?> GetRecordingStatisticsAsync(string user, int? count = null, int? offset = null,
                                                                     StatisticsRange? range = null,
                                                                     CancellationToken cancellationToken = default) {
    var address = $"stats/user/{user}/recordings";
    var options = ListenBrainz.OptionsForGetStatistics(count, offset, range);
    return this.GetOptionalAsync<IUserRecordingStatistics, UserRecordingStatistics>(address, options, cancellationToken);
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
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public IUserReleaseStatistics? GetReleaseStatistics(string user, int? count = null, int? offset = null,
                                                      StatisticsRange? range = null)
    => AsyncUtils.ResultOf(this.GetReleaseStatisticsAsync(user, count, offset, range));

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
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <returns>
  /// The requested releases statistics, or <see langword="null"/> if statistics have not yet been computed for the user.
  /// </returns>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public Task<IUserReleaseStatistics?> GetReleaseStatisticsAsync(string user, int? count = null, int? offset = null,
                                                                 StatisticsRange? range = null,
                                                                 CancellationToken cancellationToken = default) {
    var address = $"stats/user/{user}/releases";
    var options = ListenBrainz.OptionsForGetStatistics(count, offset, range);
    return this.GetOptionalAsync<IUserReleaseStatistics, UserReleaseStatistics>(address, options, cancellationToken);
  }

  #endregion

  #endregion

  #endregion

  #region /1/submit-listens

  private Task SubmitListensAsync<T>(T payload, CancellationToken cancellationToken)
    => this.PostAsync("submit-listens", payload, null, cancellationToken);

  private Task SubmitListensAsync(string payload, CancellationToken cancellationToken)
    => this.PostAsync("submit-listens", payload, null, cancellationToken);

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
  /// So the maximum listens that can be submitted at once is <c>(<see cref="MaxListenSize"/> - 36) / 72</c> (currently 141).
  /// </remarks>
  private const int MaxListensInOnePayload = (ListenBrainz.MaxListenSize - 36) / 72;

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
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public void ImportListens(IEnumerable<ISubmittedListen> listens) => AsyncUtils.ResultOf(this.ImportListensAsync(listens));

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
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public void ImportListens(params ISubmittedListen[] listens) => AsyncUtils.ResultOf(this.ImportListensAsync(listens));

  /// <summary>Imports a set of listens for the user whose token is set in <see cref="UserToken"/>.</summary>
  /// <param name="listens">The listens to import.</param>
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <return>A task that will perform the operation.</return>
  /// <remarks>
  /// This will access the <c>POST /1/submit-listens</c> endpoint.<br/>
  /// Users can find their token on their profile page:
  /// <a href="https://listenbrainz.org/profile/">https://listenbrainz.org/profile/</a>.<br/>
  /// Submissions will happen every <see cref="MaxListensInOnePayload"/> listens, and if a submission's listen data would exceed
  /// <see cref="MaxListenSize"/>, this will split them up and submit them in chunks to avoid hitting that limit. As such, one
  /// call to this method may result in multiple web service requests, which may affect rate limiting.
  /// </remarks>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public async Task ImportListensAsync(IAsyncEnumerable<ISubmittedListen> listens, CancellationToken cancellationToken = default) {
    var payload = SubmissionPayload.CreateImport();
    await foreach (var listen in listens.ConfigureAwait(false).WithCancellation(cancellationToken)) {
      payload.Listens.Add(listen);
      if (payload.Listens.Count < ListenBrainz.MaxListensInOnePayload) {
        continue;
      }
      await this.ImportListensAsync(this.SerializeImport(payload), cancellationToken).ConfigureAwait(false);
      payload.Listens.Clear();
    }
    if (payload.Listens.Count == 0) {
      return;
    }
    await this.ImportListensAsync(this.SerializeImport(payload), cancellationToken).ConfigureAwait(false);
  }

  /// <summary>Imports a set of listens for the user whose token is set in <see cref="UserToken"/>.</summary>
  /// <param name="listens">The listens to import.</param>
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <return>A task that will perform the operation.</return>
  /// <remarks>
  /// This will access the <c>POST /1/submit-listens</c> endpoint.<br/>
  /// Users can find their token on their profile page:
  /// <a href="https://listenbrainz.org/profile/">https://listenbrainz.org/profile/</a>.<br/>
  /// Submissions will happen every <see cref="MaxListensInOnePayload"/> listens, and if a submission's listen data would exceed
  /// <see cref="MaxListenSize"/>, this will split them up and submit them in chunks to avoid hitting that limit. As such, one
  /// call to this method may result in multiple web service requests, which may affect rate limiting.
  /// </remarks>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public async Task ImportListensAsync(IEnumerable<ISubmittedListen> listens, CancellationToken cancellationToken = default) {
    var payload = SubmissionPayload.CreateImport();
    foreach (var listen in listens) {
      payload.Listens.Add(listen);
      if (payload.Listens.Count < ListenBrainz.MaxListensInOnePayload) {
        continue;
      }
      await this.ImportListensAsync(this.SerializeImport(payload), cancellationToken).ConfigureAwait(false);
      payload.Listens.Clear();
    }
    if (payload.Listens.Count == 0) {
      return;
    }
    await this.ImportListensAsync(this.SerializeImport(payload), cancellationToken).ConfigureAwait(false);
  }

  private async Task ImportListensAsync(IEnumerable<string> serializedListens, CancellationToken cancellationToken = default) {
    foreach (var json in serializedListens) {
      await this.SubmitListensAsync(json, cancellationToken).ConfigureAwait(false);
    }
  }

  /// <summary>Imports a set of listens for the user whose token is set in <see cref="UserToken"/>.</summary>
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
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
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public Task ImportListensAsync(CancellationToken cancellationToken, params ISubmittedListen[] listens)
    => this.ImportListensAsync(listens, cancellationToken);

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
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public Task ImportListensAsync(params ISubmittedListen[] listens)
    => this.ImportListensAsync((IEnumerable<ISubmittedListen>) listens);

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
      foreach (var part in this.SerializeImport(partialPayload)) {
        yield return part;
      }
    }
    { // Recurse over second half
      var partialPayload = SubmissionPayload.CreateImport();
      partialPayload.Listens.AddRange(payload.Listens.GetRange(firstHalf, secondHalf));
      foreach (var part in this.SerializeImport(partialPayload)) {
        yield return part;
      }
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
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public void SetNowPlaying(ISubmittedListenData listen) => AsyncUtils.ResultOf(this.SetNowPlayingAsync(listen));

  /// <summary>Sets the "now playing" information for the user whose token is set in <see cref="UserToken"/>.</summary>
  /// <param name="track">The name of the track being listened to.</param>
  /// <param name="artist">The name of the artist performing the track being listened to.</param>
  /// <param name="release">The name of the release containing the track being listened to.</param>
  /// <remarks>
  /// This will access the <c>POST /1/submit-listens</c> endpoint.<br/>
  /// Users can find their token on their profile page:
  /// <a href="https://listenbrainz.org/profile/">https://listenbrainz.org/profile/</a>.
  /// </remarks>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public void SetNowPlaying(string track, string artist, string? release = null)
    => AsyncUtils.ResultOf(this.SetNowPlayingAsync(track, artist, release));

  /// <summary>Sets the "now playing" information for the user whose token is set in <see cref="UserToken"/>.</summary>
  /// <param name="listen">The listen data to send.</param>
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <return>A task that will perform the operation.</return>
  /// <remarks>
  /// This will access the <c>POST /1/submit-listens</c> endpoint.<br/>
  /// Users can find their token on their profile page:
  /// <a href="https://listenbrainz.org/profile/">https://listenbrainz.org/profile/</a>.
  /// </remarks>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public Task SetNowPlayingAsync(ISubmittedListenData listen, CancellationToken cancellationToken = default)
    => this.SubmitListensAsync(SubmissionPayload.CreatePlayingNow(listen), cancellationToken);

  /// <summary>Sets the "now playing" information for the user whose token is set in <see cref="UserToken"/>.</summary>
  /// <param name="track">The name of the track being listened to.</param>
  /// <param name="artist">The name of the artist performing the track being listened to.</param>
  /// <param name="release">The name of the release containing the track being listened to.</param>
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <return>A task that will perform the operation.</return>
  /// <remarks>
  /// This will access the <c>POST /1/submit-listens</c> endpoint.<br/>
  /// Users can find their token on their profile page:
  /// <a href="https://listenbrainz.org/profile/">https://listenbrainz.org/profile/</a>.
  /// </remarks>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public Task SetNowPlayingAsync(string track, string artist, string? release = null, CancellationToken cancellationToken = default)
    => this.SetNowPlayingAsync(new SubmittedListenData(track, artist, release), cancellationToken);

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
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public void SubmitSingleListen(ISubmittedListen listen) => AsyncUtils.ResultOf(this.SubmitSingleListenAsync(listen));

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
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public void SubmitSingleListen(DateTimeOffset timestamp, string track, string artist, string? release = null)
    => AsyncUtils.ResultOf(this.SubmitSingleListenAsync(timestamp, track, artist, release));

  /// <summary>
  /// Submits a single listen (typically one that has just completed) for the user whose token is set in <see cref="UserToken"/>.
  /// </summary>
  /// <param name="timestamp">
  /// The date and time at which the track was listened to, expressed as the number of seconds since
  /// <see cref="DateTimeOffset.UnixEpoch">the Unix time epoch</see>.
  /// </param>
  /// <param name="track">The name of the track being listened to.</param>
  /// <param name="artist">The name of the artist performing the track being listened to.</param>
  /// <param name="release">The name of the release containing the track being listened to.</param>
  /// <remarks>
  /// This will access the <c>POST /1/submit-listens</c> endpoint.<br/>
  /// Users can find their token on their profile page:
  /// <a href="https://listenbrainz.org/profile/">https://listenbrainz.org/profile/</a>.
  /// </remarks>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public void SubmitSingleListen(long timestamp, string track, string artist, string? release = null)
    => AsyncUtils.ResultOf(this.SubmitSingleListenAsync(timestamp, track, artist, release));

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
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public void SubmitSingleListen(string track, string artist, string? release = null)
    => AsyncUtils.ResultOf(this.SubmitSingleListenAsync(track, artist, release));

  /// <summary>
  /// Submits a single listen (typically one that has just completed) for the user whose token is set in <see cref="UserToken"/>.
  /// </summary>
  /// <param name="listen">The listen data to send.</param>
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <return>A task that will perform the operation.</return>
  /// <remarks>
  /// This will access the <c>POST /1/submit-listens</c> endpoint.<br/>
  /// Users can find their token on their profile page:
  /// <a href="https://listenbrainz.org/profile/">https://listenbrainz.org/profile/</a>.
  /// </remarks>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public Task SubmitSingleListenAsync(ISubmittedListen listen, CancellationToken cancellationToken = default)
    => this.SubmitListensAsync(SubmissionPayload.CreateSingle(listen), cancellationToken);

  /// <summary>
  /// Submits a single listen (typically one that has just completed) for the user whose token is set in <see cref="UserToken"/>.
  /// </summary>
  /// <param name="timestamp">The date and time at which the track was listened to.</param>
  /// <param name="track">The name of the track being listened to.</param>
  /// <param name="artist">The name of the artist performing the track being listened to.</param>
  /// <param name="release">The name of the release containing the track being listened to.</param>
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <return>A task that will perform the operation.</return>
  /// <remarks>
  /// This will access the <c>POST /1/submit-listens</c> endpoint.<br/>
  /// Users can find their token on their profile page:
  /// <a href="https://listenbrainz.org/profile/">https://listenbrainz.org/profile/</a>.
  /// </remarks>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public Task SubmitSingleListenAsync(DateTimeOffset timestamp, string track, string artist, string? release = null,
                                      CancellationToken cancellationToken = default)
    => this.SubmitSingleListenAsync(new SubmittedListen(timestamp, track, artist, release), cancellationToken);

  /// <summary>
  /// Submits a single listen (typically one that has just completed) for the user whose token is set in <see cref="UserToken"/>.
  /// </summary>
  /// <param name="timestamp">
  /// The date and time at which the track was listened to, expressed as the number of seconds since
  /// <see cref="DateTimeOffset.UnixEpoch">the Unix time epoch</see>.
  /// </param>
  /// <param name="track">The name of the track being listened to.</param>
  /// <param name="artist">The name of the artist performing the track being listened to.</param>
  /// <param name="release">The name of the release containing the track being listened to.</param>
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <return>A task that will perform the operation.</return>
  /// <remarks>
  /// This will access the <c>POST /1/submit-listens</c> endpoint.<br/>
  /// Users can find their token on their profile page:
  /// <a href="https://listenbrainz.org/profile/">https://listenbrainz.org/profile/</a>.
  /// </remarks>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public Task SubmitSingleListenAsync(long timestamp, string track, string artist, string? release = null,
                                      CancellationToken cancellationToken = default)
    => this.SubmitSingleListenAsync(new SubmittedListen(timestamp, track, artist, release), cancellationToken);

  /// <summary>
  /// Submits a single listen for the user whose token is set in <see cref="UserToken"/>, using the current (UTC) date and time as
  /// timestamp.
  /// </summary>
  /// <param name="track">The name of the track being listened to.</param>
  /// <param name="artist">The name of the artist performing the track being listened to.</param>
  /// <param name="release">The name of the release containing the track being listened to.</param>
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <remarks>
  /// This will access the <c>POST /1/submit-listens</c> endpoint.<br/>
  /// Users can find their token on their profile page:
  /// <a href="https://listenbrainz.org/profile/">https://listenbrainz.org/profile/</a>.
  /// </remarks>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public Task SubmitSingleListenAsync(string track, string artist, string? release = null,
                                      CancellationToken cancellationToken = default)
    => this.SubmitSingleListenAsync(new SubmittedListen(track, artist, release), cancellationToken);

  #endregion

  #endregion

  #region /1/user/xxx/listen-count

  /// <summary>Gets the number of listens submitted to ListenBrainz by a particular user.</summary>
  /// <param name="user">The MusicBrainz ID of the user whose listen count is requested.</param>
  /// <returns>An object providing the number of listens submitted by <paramref name="user"/>.</returns>
  /// <remarks>This will access the <c>GET /1/user/USER/listen-count</c> endpoint.</remarks>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public IListenCount GetListenCount(string user) => AsyncUtils.ResultOf(this.GetListenCountAsync(user));

  /// <summary>Gets the number of listens submitted to ListenBrainz by a particular user.</summary>
  /// <param name="user">The MusicBrainz ID of the user whose listen count is requested.</param>
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <returns>An object providing the number of listens submitted by <paramref name="user"/>.</returns>
  /// <remarks>This will access the <c>GET /1/user/USER/listen-count</c> endpoint.</remarks>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public Task<IListenCount> GetListenCountAsync(string user, CancellationToken cancellationToken = default)
    => this.GetAsync<IListenCount, ListenCount>($"user/{user}/listen-count", null, cancellationToken);

  #endregion

  #region /1/user/xxx/listens

  #region Internal Helpers

  private static IDictionary<string, string> OptionsForGetListens(int? count, long? after, long? before, int? timeRange) {
    var options = new Dictionary<string, string>(4);
    if (count is not null) {
      options.Add("count", count.Value.ToString(CultureInfo.InvariantCulture));
    }
    if (before is not null) {
      options.Add("max_ts", before.Value.ToString(CultureInfo.InvariantCulture));
    }
    if (after is not null) {
      options.Add("min_ts", after.Value.ToString(CultureInfo.InvariantCulture));
    }
    if (timeRange is not null) {
      options.Add("time_range", timeRange.Value.ToString(CultureInfo.InvariantCulture));
    }
    return options;
  }

  private IFetchedListens PerformGetListens(string user, long? after, long? before, int? count = null, int? timeRange = null)
    => AsyncUtils.ResultOf(this.PerformGetListensAsync(user, after, before, count, timeRange));

  private Task<IFetchedListens> PerformGetListensAsync(string user, long? after, long? before, int? count = null,
                                                       int? timeRange = null, CancellationToken cancellationToken = default) {
    var options = ListenBrainz.OptionsForGetListens(count, after, before, timeRange);
    return this.GetAsync<IFetchedListens, FetchedListens>($"user/{user}/listens", options, cancellationToken);
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
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
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
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <returns>The requested listens, in descending timestamp order.</returns>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public Task<IFetchedListens> GetListensAsync(string user, int? count = null, int? timeRange = null,
                                               CancellationToken cancellationToken = default)
    => this.PerformGetListensAsync(user, null, null, count, timeRange, cancellationToken);

  #endregion

  #region Minimum Timestamp

  /// <summary>Gets the most recent listens for a user, starting from a particular timestamp.</summary>
  /// <param name="user">The MusicBrainz ID of the user whose data is needed.</param>
  /// <param name="after">
  /// The timestamp to start from, expressed as the number of seconds since
  /// <see cref="DateTimeOffset.UnixEpoch">the Unix time epoch</see>. Returned listens will have a timestamp greater than, but not
  /// including, this value.
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
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
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
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public IFetchedListens GetListensAfter(string user, DateTimeOffset after, int? count = null, int? timeRange = null)
    => this.PerformGetListens(user, after.ToUnixTimeSeconds(), null, count, timeRange);

  /// <summary>Gets the most recent listens for a user, starting from a particular timestamp.</summary>
  /// <param name="user">The MusicBrainz ID of the user whose data is needed.</param>
  /// <param name="after">
  /// The timestamp to start from, expressed as the number of seconds since
  /// <see cref="DateTimeOffset.UnixEpoch">the Unix time epoch</see>. Returned listens will have a timestamp greater than, but not
  /// including, this value.
  /// </param>
  /// <param name="count">
  /// The (maximum) number of listens to return; must be no greater than <see cref="MaxItemsPerGet"/>.<br/>
  /// If not specified, this will return up to <see cref="DefaultItemsPerGet"/> listens.
  /// </param>
  /// <param name="timeRange">
  /// The time range to search, in sets of 5 days; must be no greater than <see cref="MaxTimeRange"/>.<br/>
  /// If not specified, <see cref="DefaultTimeRange"/> will be used as time range.
  /// </param>
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <returns>The requested listens, in descending timestamp order.</returns>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public Task<IFetchedListens> GetListensAfterAsync(string user, long after, int? count = null, int? timeRange = null,
                                                    CancellationToken cancellationToken = default)
    => this.PerformGetListensAsync(user, after, null, count, timeRange, cancellationToken);

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
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <returns>The requested listens, in descending timestamp order.</returns>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public Task<IFetchedListens> GetListensAfterAsync(string user, DateTimeOffset after, int? count = null,
                                                    int? timeRange = null, CancellationToken cancellationToken = default)
    => this.PerformGetListensAsync(user, after.ToUnixTimeSeconds(), null, count, timeRange, cancellationToken);

  #endregion

  #region Maximum Timestamp

  /// <summary>Gets historical listens for a user, starting from a particular timestamp.</summary>
  /// <param name="user">The MusicBrainz ID of the user whose data is needed.</param>
  /// <param name="before">
  /// The timestamp to start from, expressed as the number of seconds since
  /// <see cref="DateTimeOffset.UnixEpoch">the Unix time epoch</see>. Returned listens will have a timestamp less than, but not
  /// including, this value.
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
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
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
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public IFetchedListens GetListensBefore(string user, DateTimeOffset before, int? count = null, int? timeRange = null)
    => this.PerformGetListens(user, null, before.ToUnixTimeSeconds(), count, timeRange);

  /// <summary>Gets historical listens for a user, starting from a particular timestamp.</summary>
  /// <param name="user">The MusicBrainz ID of the user whose data is needed.</param>
  /// <param name="before">
  /// The timestamp to start from, expressed as the number of seconds since
  /// <see cref="DateTimeOffset.UnixEpoch">the Unix time epoch</see>. Returned listens will have a timestamp less than, but not
  /// including, this value.
  /// </param>
  /// <param name="count">
  /// The (maximum) number of listens to return; must be no greater than <see cref="MaxItemsPerGet"/>.<br/>
  /// If not specified, this will return up to <see cref="DefaultItemsPerGet"/> listens.
  /// </param>
  /// <param name="timeRange">
  /// The time range to search, in sets of 5 days; must be no greater than <see cref="MaxTimeRange"/>.<br/>
  /// If not specified, <see cref="DefaultTimeRange"/> will be used as time range.
  /// </param>
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <returns>The requested listens, in descending timestamp order.</returns>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public Task<IFetchedListens> GetListensBeforeAsync(string user, long before, int? count = null, int? timeRange = null,
                                                     CancellationToken cancellationToken = default)
    => this.PerformGetListensAsync(user, null, before, count, timeRange, cancellationToken);

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
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <returns>The requested listens, in descending timestamp order.</returns>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public Task<IFetchedListens> GetListensBeforeAsync(string user, DateTimeOffset before, int? count = null,
                                                     int? timeRange = null, CancellationToken cancellationToken = default)
    => this.PerformGetListensAsync(user, null, before.ToUnixTimeSeconds(), count, timeRange, cancellationToken);

  #endregion

  #region Both Timestamps

  /// <summary>Gets the listens for a user in a specific timespan.</summary>
  /// <param name="user">The MusicBrainz ID of the user whose data is needed.</param>
  /// <param name="after">
  /// The timestamp to start from, expressed as the number of seconds since
  /// <see cref="DateTimeOffset.UnixEpoch">the Unix time epoch</see>. Returned listens will have a timestamp greater than, but not
  /// including, this value.
  /// </param>
  /// <param name="before">
  /// The timestamp to end at, expressed as the number of seconds since
  /// <see cref="DateTimeOffset.UnixEpoch">the Unix time epoch</see>. Returned listens will have a timestamp less than, but not
  /// including, this value.
  /// </param>
  /// <param name="count">
  /// The (maximum) number of listens to return; must be no greater than <see cref="MaxItemsPerGet"/>.<br/>
  /// If not specified, this will return up to <see cref="DefaultItemsPerGet"/> listens.
  /// </param>
  /// <returns>The requested listens, in descending timestamp order.</returns>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
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
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public IFetchedListens GetListensBetween(string user, DateTimeOffset after, DateTimeOffset before, int? count = null)
    => this.PerformGetListens(user, after.ToUnixTimeSeconds(), before.ToUnixTimeSeconds(), count);

  /// <summary>Gets the listens for a user in a specific timespan.</summary>
  /// <param name="user">The MusicBrainz ID of the user whose data is needed.</param>
  /// <param name="after">
  /// The timestamp to start from, expressed as the number of seconds since
  /// <see cref="DateTimeOffset.UnixEpoch">the Unix time epoch</see>. Returned listens will have a timestamp greater than, but not
  /// including, this value.
  /// </param>
  /// <param name="before">
  /// The timestamp to end at, expressed as the number of seconds since
  /// <see cref="DateTimeOffset.UnixEpoch">the Unix time epoch</see>. Returned listens will have a timestamp less than, but not
  /// including, this value.
  /// </param>
  /// <param name="count">
  /// The (maximum) number of listens to return; must be no greater than <see cref="MaxItemsPerGet"/>.<br/>
  /// If not specified, this will return up to <see cref="DefaultItemsPerGet"/> listens.
  /// </param>
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <returns>The requested listens, in descending timestamp order.</returns>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public Task<IFetchedListens> GetListensBetweenAsync(string user, long after, long before, int? count = null,
                                                      CancellationToken cancellationToken = default)
    => this.PerformGetListensAsync(user, after, before, count, null, cancellationToken);

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
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <returns>The requested listens, in descending timestamp order.</returns>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public Task<IFetchedListens> GetListensBetweenAsync(string user, DateTimeOffset after, DateTimeOffset before, int? count = null,
                                                      CancellationToken cancellationToken = default)
    => this.PerformGetListensAsync(user, after.ToUnixTimeSeconds(), before.ToUnixTimeSeconds(), count, null, cancellationToken);

  #endregion

  #endregion

  #region /1/user/xxx/playing-now

  /// <summary>Gets a user's currently-playing listen(s).</summary>
  /// <param name="user">The MusicBrainz ID of the user whose data is needed.</param>
  /// <returns>The requested listens (typically 0 or 1).</returns>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public IPlayingNow GetPlayingNow(string user) => AsyncUtils.ResultOf(this.GetPlayingNowAsync(user));

  /// <summary>Gets a user's currently-playing listen(s).</summary>
  /// <param name="user">The MusicBrainz ID of the user whose data is needed.</param>
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <returns>The requested listens (typically 0 or 1).</returns>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public Task<IPlayingNow> GetPlayingNowAsync(string user, CancellationToken cancellationToken = default)
    => this.GetAsync<IPlayingNow, PlayingNow>($"user/{user}/playing-now", null, cancellationToken);

  #endregion

  #region /1/validate-token

  private static Dictionary<string, string> OptionsForTokenValidation(string token) => new() { ["token"] = token };

  /// <summary>Validates a given user token.</summary>
  /// <param name="token">The user token to validate.</param>
  /// <returns>The result of the validation.</returns>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public ITokenValidationResult ValidateToken(string token) => AsyncUtils.ResultOf(this.ValidateTokenAsync(token));

  /// <summary>Validates a given user token.</summary>
  /// <param name="token">The user token to validate.</param>
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <returns>The result of the validation.</returns>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public Task<ITokenValidationResult> ValidateTokenAsync(string token, CancellationToken cancellationToken = default) {
    var options = ListenBrainz.OptionsForTokenValidation(token);
    return this.GetAsync<ITokenValidationResult, TokenValidationResult>("validate-token", options, cancellationToken);
  }

  #endregion

  #endregion

  #region HTTP Client / IDisposable

  private static readonly MediaTypeWithQualityHeaderValue AcceptHeader = new("application/json");

  private static readonly ProductInfoHeaderValue LibraryComment = new($"({ListenBrainz.UserAgentUrl})");

  private static readonly ProductInfoHeaderValue LibraryProductInfo = HttpUtils.CreateUserAgentHeader<ListenBrainz>();

  private AuthenticationHeaderValue? _authorization;

  private HttpClient? _client;

  private Action<HttpClient>? _clientConfiguration;

  private Func<HttpClient>? _clientCreation;

  private readonly bool _clientOwned;

  private bool _disposed;

  private readonly List<ProductInfoHeaderValue> _userAgent = new(ListenBrainz.DefaultUserAgent);

  private HttpClient Client {
    get {
#if NET6_0
      if (this._disposed) {
        throw new ObjectDisposedException(nameof(ListenBrainz));
      }
#else
      ObjectDisposedException.ThrowIf(this._disposed, typeof(ListenBrainz));
#endif
      if (this._client is null) {
        var client = this._clientCreation?.Invoke() ?? new HttpClient();
        this._userAgent.ForEach(client.DefaultRequestHeaders.UserAgent.Add);
        this._clientConfiguration?.Invoke(client);
        this._client = client;
      }
      return this._client;
    }
  }

  /// <summary>Closes the underlying web service client in use by this ListenBrainz client, if there is one.</summary>
  /// <remarks>The next web service request will create a new client.</remarks>
  /// <exception cref="InvalidOperationException">When this instance is using an explicitly provided client instance.</exception>
  public void Close() {
    if (!this._clientOwned) {
      throw new InvalidOperationException("An explicitly provided client instance is in use.");
    }
    Interlocked.Exchange(ref this._client, null)?.Dispose();
  }

  /// <summary>Sets up code to run to configure a newly-created HTTP client.</summary>
  /// <param name="code">The configuration code for an HTTP client, or <see langword="null"/> to clear such code.</param>
  /// <remarks>The configuration code will be called <em>after</em> <see cref="UserAgent"/> is applied.</remarks>
  public void ConfigureClient(Action<HttpClient>? code) {
    this._clientConfiguration = code;
  }

  /// <summary>Sets up code to run to create an HTTP client.</summary>
  /// <param name="code">The creation code for an HTTP client, or <see langword="null"/> to clear such code.</param>
  /// <remarks>
  /// <see cref="UserAgent"/> and any code set via <see cref="ConfigureClient(System.Action{System.Net.Http.HttpClient}?)"/> will be
  /// applied to the client returned by <paramref name="code"/>.
  /// </remarks>
  public void ConfigureClientCreation(Func<HttpClient>? code) {
    this._clientCreation = code;
  }

  /// <summary>Discards any and all resources held by this ListenBrainz client.</summary>
  /// <remarks>Further attempts at web service requests will cause <see cref="ObjectDisposedException"/> to be thrown.</remarks>
  public void Dispose() {
    this.Dispose(true);
    GC.SuppressFinalize(this);
  }

  private void Dispose(bool disposing) {
    if (!disposing) {
      // no unmanaged resources
      return;
    }
    try {
      if (this._clientOwned) {
        this.Close();
      }
      this._client = null;
    }
    finally {
      this._disposed = true;
    }
  }

  /// <summary>Finalizes this instance, releasing any and all resources.</summary>
  ~ListenBrainz() {
    this.Dispose(false);
  }

  #endregion

  #region Internals

  #region JSON Options

  private static readonly JsonSerializerOptions JsonReaderOptions = JsonUtils.CreateReaderOptions(Converters.Readers);

  private static readonly JsonSerializerOptions JsonWriterOptions = JsonUtils.CreateWriterOptions(Converters.Writers);

  #endregion

  #region Basic Request Execution

  private Uri BuildUri(string path, string? extra = null)
    => new UriBuilder(this.UrlScheme, this.Server, this.Port, ListenBrainz.WebServiceRoot + path, extra).Uri;

  private async Task<TInterface> GetAsync<TInterface, TObject>(string address, IDictionary<string, string>? options,
                                                               CancellationToken cancellationToken = default)
  where TInterface : class
  where TObject : class, TInterface {
    var response = await this.PerformRequestAsync(address, HttpMethod.Get, null, options, cancellationToken).ConfigureAwait(false);
    var task = JsonUtils.GetJsonContentAsync<TObject>(response, ListenBrainz.JsonReaderOptions, cancellationToken);
    return await task.ConfigureAwait(false);
  }

  private async Task<TInterface?> GetOptionalAsync<TInterface, TObject>(string address, IDictionary<string, string>? options,
                                                                        CancellationToken cancellationToken = default)
  where TInterface : class
  where TObject : class, TInterface {
    var response = await this.PerformRequestAsync(address, HttpMethod.Get, null, options, cancellationToken).ConfigureAwait(false);
    if (response.StatusCode == HttpStatusCode.NoContent) {
      return null;
    }
    var task = JsonUtils.GetJsonContentAsync<TObject>(response, ListenBrainz.JsonReaderOptions, cancellationToken);
    return await task.ConfigureAwait(false);
  }

  private async Task<HttpResponseMessage> PerformRequestAsync(string endPoint, HttpMethod method, HttpContent? body,
                                                              IDictionary<string, string>? options,
                                                              CancellationToken cancellationToken = default) {
    var request = new HttpRequestMessage(method, this.BuildUri(endPoint, ListenBrainz.QueryString(options)));
    var ts = ListenBrainz.TraceSource;
    ts.TraceEvent(TraceEventType.Verbose, 1, "WEB SERVICE REQUEST: {0} {1}", method.Method, request.RequestUri);
    var client = this.Client;
    {
      var headers = request.Headers;
      headers.Accept.Add(ListenBrainz.AcceptHeader);
      headers.Authorization = this._authorization;
      // Use whatever user agent the client has set, plus our own.
      {
        var userAgent = headers.UserAgent;
        foreach (var ua in client.DefaultRequestHeaders.UserAgent) {
          userAgent.Add(ua);
        }
        userAgent.Add(ListenBrainz.LibraryProductInfo);
        userAgent.Add(ListenBrainz.LibraryComment);
      }
    }
    if (ts.Switch.ShouldTrace(TraceEventType.Verbose)) {
      ts.TraceEvent(TraceEventType.Verbose, 2, "HEADERS: {0}", TextUtils.FormatMultiLine(request.Headers.ToString()));
      if (request.Content is not null) {
        var headers = request.Content.Headers;
        ts.TraceEvent(TraceEventType.Verbose, 3, "BODY ({0}, {1} bytes): {2}", headers.ContentType, headers.ContentLength ?? 0,
                      TextUtils.FormatMultiLine(await request.Content.ReadAsStringAsync(cancellationToken)));
      }
      else {
        ts.TraceEvent(TraceEventType.Verbose, 3, "NO BODY");
      }
    }
    request.Content = body;
    var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);
    if (ts.Switch.ShouldTrace(TraceEventType.Verbose)) {
      ts.TraceEvent(TraceEventType.Verbose, 4, "WEB SERVICE RESPONSE: {0:D}/{0} '{1}' (v{2})", response.StatusCode,
                    response.ReasonPhrase, response.Version);
      ts.TraceEvent(TraceEventType.Verbose, 5, "HEADERS: {0}", TextUtils.FormatMultiLine(response.Headers.ToString()));
      var headers = response.Content.Headers;
      ts.TraceEvent(TraceEventType.Verbose, 6, "CONTENT ({0}): {1} bytes", headers.ContentType, headers.ContentLength ?? 0);
    }
    var rateLimitInfo = new RateLimitInfo(response.Headers);
    this._rateLimitLock.EnterWriteLock();
    try {
      this._rateLimitInfo = rateLimitInfo;
    }
    finally {
      this._rateLimitLock.ExitWriteLock();
    }
    try {
      return await response.EnsureSuccessfulAsync(cancellationToken).ConfigureAwait(false);
    }
    catch (HttpError error) {
      // If we get an error with content that can be interpreted as an ErrorInfo structure, wrap it in an error containing that info
      if (!string.IsNullOrEmpty(error.Content)) {
        ErrorInfo? ei;
        try {
          ei = JsonSerializer.Deserialize<ErrorInfo>(error.Content, ListenBrainz.JsonReaderOptions);
          if (ei is null) {
            throw new JsonException("Error info was null.");
          }
        }
        catch (Exception e) {
          ts.TraceEvent(TraceEventType.Verbose, 7, "FAILED TO PARSE ERROR RESPONSE CONTENT AS JSON: {0}", e.Message);
          ei = null;
        }
        if (ei is not null) {
          var reason = error.Reason;
          if (ei.Code != (int) response.StatusCode) {
            ts.TraceEvent(TraceEventType.Verbose, 8, "ERROR CODE ({0}) DOES NOT MATCH HTTP STATUS CODE", ei.Code);
            reason = "Error";
          }
          if (ei.UnhandledProperties is not null) {
            foreach (var prop in ei.UnhandledProperties) {
              ts.TraceEvent(TraceEventType.Verbose, 9, "UNEXPECTED ERROR PROPERTY: {0} -> {1}", prop.Key, prop.Value);
            }
          }
          throw new HttpError((HttpStatusCode) ei.Code, reason, response.Version, ei.Error, error);
        }
      }
      throw;
    }
  }

  private Task PostAsync<T>(string address, T content, IDictionary<string, string>? options,
                            CancellationToken cancellationToken = default)
    => this.PostAsync(address, JsonSerializer.Serialize(content, ListenBrainz.JsonWriterOptions), options, cancellationToken);

  private async Task PostAsync(string address, string body, IDictionary<string, string>? options,
                               CancellationToken cancellationToken = default) {
    var content = new StringContent(body, Encoding.UTF8, "application/json");
    var performRequest = this.PerformRequestAsync(address, HttpMethod.Post, content, options, cancellationToken);
    var response = await performRequest.ConfigureAwait(false);
    if (ListenBrainz.TraceSource.Switch.ShouldTrace(TraceEventType.Verbose)) {
      var message = await response.GetStringContentAsync(cancellationToken).ConfigureAwait(false);
      if (message.Length > 0) {
        ListenBrainz.TraceSource.TraceEvent(TraceEventType.Verbose, 9, "MESSAGE: {0}", TextUtils.FormatMultiLine(message));
      }
    }
  }

  #endregion

  #region Utility Methods

  private static string QueryString(IDictionary<string, string>? options) {
    if (options is null || options.Count == 0) {
      return "";
    }
    var sb = new StringBuilder();
    var separator = '?';
    foreach (var option in options) {
      // FIXME: Which parts (if any) need URL/Data escaping?
      sb.Append(separator).Append(option.Key).Append('=').Append(option.Value);
      separator = '&';
    }
    return sb.ToString();
  }

  #endregion

  #endregion

}
