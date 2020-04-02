using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

using JetBrains.Annotations;

using MetaBrainz.Common.Json;
using MetaBrainz.Common.Json.Converters;
using MetaBrainz.ListenBrainz.Interfaces;
using MetaBrainz.ListenBrainz.Objects;

namespace MetaBrainz.ListenBrainz {

  /// <summary>Main class for accessing the ListenBrainz API.</summary>
  [PublicAPI]
  public sealed class ListenBrainz : IDisposable {

    #region Constants

    /// <summary>The default number of listens returned in a single GET request.</summary>
    public const int DefaultItemsPerGet = 25;

    /// <summary>The maximum number of listens returned in a single GET request.</summary>
    public const int MaxItemsPerGet = 100;

    /// <summary>Maximum overall listen size in bytes, to prevent egregious spamming.</summary>
    public const int MaxListenSize = 10240;

    /// <summary>The maximum length of a tag.</summary>
    public const int MaxTagLength = 64;

    /// <summary>The maximum number of tags per listen.</summary>
    public const int MaxTagsPerListen = 50;

    /// <summary>The URL included in the user agent for requests as part of this library's information.</summary>
    public const string UserAgentUrl = "https://github.com/Zastai/ListenBrainz";

    /// <summary>The root location of the web service.</summary>
    public const string WebServiceRoot = "/1/";

    #endregion

    #region Static Fields / Properties

    /// <summary>The default port number to use for requests (-1 to not specify any explicit port).</summary>
    public static int DefaultPort { get; set; } = -1;

    /// <summary>The default server to use for requests.</summary>
    public static string DefaultServer { get; set; } = "api.listenbrainz.org";

    /// <summary>The default internet access protocol to use for requests.</summary>
    public static string DefaultUrlScheme { get; set; } = "https";

    /// <summary>The default user agent to use for requests.</summary>
    public static string? DefaultUserAgent { get; set; }

    /// <summary>The default user token to use for requests; used as initial value for <see cref="UserToken"/>.</summary>
    public static string? DefaultUserToken { get; set; }

    #endregion

    #region Constructors

    /// <summary>Creates a new instance of the <see cref="T:ListenBrainz"/> class.</summary>
    /// <param name="userAgent">The user agent to use for all requests.</param>
    /// <exception cref="ArgumentException">
    /// When the user agent (whether from <paramref name="userAgent"/> or <see cref="DefaultUserAgent"/>) is blank.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// When <paramref name="userAgent"/> is <see langword="null"/>, and no default was set via <see cref="DefaultUserAgent"/>.
    /// </exception>
    public ListenBrainz(string? userAgent = null) {
      userAgent ??= ListenBrainz.DefaultUserAgent;
      if (userAgent == null)
        throw new ArgumentNullException(nameof(userAgent));
      if (string.IsNullOrWhiteSpace(userAgent))
        throw new ArgumentException("The user agent must not be blank.", nameof(userAgent));
      this.UserAgent = userAgent;
      { // Set full user agent, including this library's information
        var an = typeof(ListenBrainz).Assembly.GetName();
        this._fullUserAgent = $"{this.UserAgent} {an.Name}/{an.Version} ({ListenBrainz.UserAgentUrl})";
      }
      this.UserToken = ListenBrainz.DefaultUserToken;
    }

    /// <summary>Creates a new instance of the <see cref="T:ListenBrainz" /> class.</summary>
    /// <param name="application">The application name to use in the user agent property for all requests.</param>
    /// <param name="version">The version number to use in the user agent property for all requests.</param>
    /// <param name="contact">
    /// The contact address (typically HTTP, HTTPS or MAILTO) to use in the user agent property for all requests.
    /// </param>
    /// <exception cref="T:System.ArgumentException">When <paramref name="application"/> is blank.</exception>
    public ListenBrainz(string application, Version version, Uri contact)
    : this(application, version.ToString(), contact.ToString())
    { }

    /// <summary>Creates a new instance of the <see cref="T:ListenBrainz"/> class.</summary>
    /// <param name="application">The application name to use in the user agent property for all requests.</param>
    /// <param name="version">The version number to use in the user agent property for all requests.</param>
    /// <param name="contact">
    /// The contact address (typically a URL or email address) to use in the user agent property for all requests.
    /// </param>
    /// <exception cref="ArgumentException">
    /// When <paramref name="application"/>, <paramref name="version"/> and/or <paramref name="contact"/> are blank.
    /// </exception>
    public ListenBrainz(string application, string version, string contact) {
      if (string.IsNullOrWhiteSpace(application))
        throw new ArgumentException("The application name must not be blank.", nameof(application));
      if (string.IsNullOrWhiteSpace(version))
        throw new ArgumentException("The version number must not be blank.", nameof(version));
      if (string.IsNullOrWhiteSpace(contact))
        throw new ArgumentException("The contact address must not be blank.", nameof(contact));
      this.UserAgent = $"{application}/{version} ({contact})";
      { // Set full user agent, including this library's information
        var an = typeof(ListenBrainz).Assembly.GetName();
        this._fullUserAgent = $"{this.UserAgent} {an.Name}/{an.Version} ({ListenBrainz.UserAgentUrl})";
      }
      this.UserToken = ListenBrainz.DefaultUserToken;
    }

    #endregion

    #region Public Instance Fields / Properties

    /// <summary>The base URI for all requests.</summary>
    public Uri BaseUri => new UriBuilder(this.UrlScheme, this.Server, this.Port, ListenBrainz.WebServiceRoot).Uri;

    /// <summary>The port number to use for requests (-1 to not specify any explicit port).</summary>
    public int Port { get; set; } = ListenBrainz.DefaultPort;

    /// <summary>The server to use for requests.</summary>
    public string Server { get; set; } = ListenBrainz.DefaultServer;

    /// <summary>The internet access protocol to use for requests.</summary>
    public string UrlScheme { get; set; } = ListenBrainz.DefaultUrlScheme;

    /// <summary>The user agent to use for requests.</summary>
    public string UserAgent { get; }

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
    public string? UserToken { get; }

    #endregion

    #region Public API

    /// <summary>Information about the active rate limiting. Gets refreshed after every API call.</summary>
    public RateLimitInfo RateLimitInfo { get; private set; }

    #region /1/latest-import

    private static NameValueCollection OptionsForLatestImport(string user) {
      var options = new NameValueCollection(1);
      options.Set("user_name", user);
      return options;
    }

    /// <summary>Get the timestamp of the newest listen submitted by a user in previous imports to ListenBrainz.</summary>
    /// <param name="user">The MusicBrainz ID of the user whose data is requested.</param>
    /// <returns>An object providing the user's ID and latest import timestamp.</returns>
    /// <remarks>This will access the <c>GET /1/latest-import</c> endpoint.</remarks>
    public ILatestImport GetLatestImport(string user) {
      var json = this.PerformRequest("latest-import", Method.Get, ListenBrainz.OptionsForLatestImport(user));
      return JsonUtils.Deserialize<LatestImport>(json, ListenBrainz.JsonOptionsForRead);
    }

    /// <summary>Get the timestamp of the newest listen submitted by a user in previous imports to ListenBrainz.</summary>
    /// <param name="user">The MusicBrainz ID of the user whose data is requested.</param>
    /// <returns>A task returning an object providing the user's ID and latest import timestamp.</returns>
    /// <remarks>This will access the <c>GET /1/latest-import</c> endpoint.</remarks>
    public async Task<ILatestImport> GetLatestImportAsync(string user) {
      var task = this.PerformRequestAsync("latest-import", Method.Get, ListenBrainz.OptionsForLatestImport(user));
      var json = await task.ConfigureAwait(false);
      return JsonUtils.Deserialize<LatestImport>(json, ListenBrainz.JsonOptionsForRead);
    }

    /// <summary>Set the timestamp of the newest listen submitted by a user in previous imports to ListenBrainz.</summary>
    /// <param name="user">The MusicBrainz ID of the user whose data should be modified.</param>
    /// <param name="timestamp">The timestamp to set.</param>
    /// <remarks>
    /// This will access the <c>POST /1/latest-import</c> endpoint and requires <see cref="UserToken"/> to be set to the token
    /// for <paramref name="user"/>.
    /// </remarks>
    public void SetLatestImport(string user, DateTimeOffset timestamp) {
      this.SetLatestImport(user, UnixTime.Convert(timestamp));
    }

    /// <summary>Set the timestamp of the newest listen submitted by a user in previous imports to ListenBrainz.</summary>
    /// <param name="user">The MusicBrainz ID of the user whose data is needed.</param>
    /// <param name="timestamp">
    /// The timestamp to set, expressed as the number of seconds since <see cref="UnixTime.Epoch">the Unix time epoch</see>.
    /// </param>
    /// <remarks>
    /// This will access the <c>POST /1/latest-import</c> endpoint and requires <see cref="UserToken"/> to be set to the token
    /// for <paramref name="user"/>.
    /// </remarks>
    public void SetLatestImport(string user, long timestamp) {
      this.PerformRequest("latest-import", Method.Post, $"{{ ts: {timestamp} }}", ListenBrainz.OptionsForLatestImport(user));
    }

    /// <summary>Set the timestamp of the newest listen submitted by a user in previous imports to ListenBrainz.</summary>
    /// <param name="user">The MusicBrainz ID of the user whose data should be modified.</param>
    /// <param name="timestamp">The timestamp to set.</param>
    /// <return>A task that will perform the operation.</return>
    /// <remarks>
    /// This will access the <c>POST /1/latest-import</c> endpoint.<br/>
    /// Users can find their token on their profile page:
    /// <a href="https://listenbrainz.org/profile/">https://listenbrainz.org/profile/</a>.
    /// </remarks>
    public Task SetLatestImportAsync(string user, DateTimeOffset timestamp) {
      return this.SetLatestImportAsync(user, UnixTime.Convert(timestamp));
    }

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
    public async Task SetLatestImportAsync(string user, long timestamp) {
      var options = ListenBrainz.OptionsForLatestImport(user);
      var task = this.PerformRequestAsync("latest-import", Method.Post, $"{{ ts: {timestamp} }}", options);
      await task.ConfigureAwait(false);
    }

    #endregion

    #region /1/submit-listens

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
    /// If the listen data would exceed <see cref="MaxListenSize"/>, this will split them up and submit them in chunks to avoid
    /// hitting that limit. As such, one call to this method may result in multiple web service requests, which may affect rate
    /// limiting.
    /// </remarks>
    public void ImportListens(IEnumerable<ISubmittedListen> listens) {
      var payload = SubmissionPayload.CreateImport();
      // FIXME: Should this use foreach instead and forcibly perform a submission after MaxListensInOnePayload listens were added?
      // FIXME: That way, it could support a huge range of listens coming from the enumerable in a streaming fashion.
      payload.Listens.AddRange(listens);
      foreach (var json in this.SerializeImport(payload))
        this.PerformRequest("submit-listens", Method.Post, json);
    }

    /// <summary>Imports a set of listens for the user whose token is set in <see cref="UserToken"/>.</summary>
    /// <param name="listens">The listens to import.</param>
    /// <remarks>
    /// This will access the <c>POST /1/submit-listens</c> endpoint.<br/>
    /// Users can find their token on their profile page:
    /// <a href="https://listenbrainz.org/profile/">https://listenbrainz.org/profile/</a>.<br/>
    /// If the listen data would exceed <see cref="MaxListenSize"/>, this will split them up and submit them in chunks to avoid
    /// hitting that limit. As such, one call to this method may result in multiple web service requests, which may affect rate
    /// limiting.
    /// </remarks>
    public void ImportListens(params ISubmittedListen[] listens) {
      var payload = SubmissionPayload.CreateImport();
      payload.Listens.AddRange(listens);
      foreach (var json in this.SerializeImport(payload))
        this.PerformRequest("submit-listens", Method.Post, json);
    }

    /// <summary>Imports a set of listens for the user whose token is set in <see cref="UserToken"/>.</summary>
    /// <param name="listens">The listens to import.</param>
    /// <return>A task that will perform the operation.</return>
    /// <remarks>
    /// This will access the <c>POST /1/submit-listens</c> endpoint.<br/>
    /// Users can find their token on their profile page:
    /// <a href="https://listenbrainz.org/profile/">https://listenbrainz.org/profile/</a>.<br/>
    /// If the listen data would exceed <see cref="MaxListenSize"/>, this will split them up and submit them in chunks to avoid
    /// hitting that limit. As such, one call to this method may result in multiple web service requests, which may affect rate
    /// limiting.
    /// </remarks>
    public async Task ImportListensAsync(IAsyncEnumerable<ISubmittedListen> listens) {
      var payload = SubmissionPayload.CreateImport();
      // FIXME: Should this forcibly perform a submission after MaxListensInOnePayload listens were added?
      // FIXME: That way, it could support a huge range of listens coming from the enumerable in a streaming fashion.
      await foreach(var listen in listens)
        payload.Listens.Add(listen);
      foreach (var json in this.SerializeImport(payload)) {
        var task = this.PerformRequestAsync("submit-listens", Method.Post, json);
        await task.ConfigureAwait(false);
      }
    }

    /// <summary>Imports a set of listens for the user whose token is set in <see cref="UserToken"/>.</summary>
    /// <param name="listens">The listens to import.</param>
    /// <return>A task that will perform the operation.</return>
    /// <remarks>
    /// This will access the <c>POST /1/submit-listens</c> endpoint.<br/>
    /// Users can find their token on their profile page:
    /// <a href="https://listenbrainz.org/profile/">https://listenbrainz.org/profile/</a>.<br/>
    /// If the listen data would exceed <see cref="MaxListenSize"/>, this will split them up and submit them in chunks to avoid
    /// hitting that limit. As such, one call to this method may result in multiple web service requests, which may affect rate
    /// limiting.
    /// </remarks>
    public async Task ImportListensAsync(IEnumerable<ISubmittedListen> listens) {
      var payload = SubmissionPayload.CreateImport();
      // FIXME: Should this use foreach instead and forcibly perform a submission after MaxListensInOnePayload listens were added?
      // FIXME: That way, it could support a huge range of listens coming from the enumerable in a streaming fashion.
      payload.Listens.AddRange(listens);
      foreach (var json in this.SerializeImport(payload)) {
        var task = this.PerformRequestAsync("submit-listens", Method.Post, json);
        await task.ConfigureAwait(false);
      }
    }

    /// <summary>Imports a set of listens for the user whose token is set in <see cref="UserToken"/>.</summary>
    /// <param name="listens">The listens to import.</param>
    /// <return>A task that will perform the operation.</return>
    /// <remarks>
    /// This will access the <c>POST /1/submit-listens</c> endpoint.<br/>
    /// Users can find their token on their profile page:
    /// <a href="https://listenbrainz.org/profile/">https://listenbrainz.org/profile/</a>.<br/>
    /// If the listen data would exceed <see cref="MaxListenSize"/>, this will split them up and submit them in chunks to avoid
    /// hitting that limit. As such, one call to this method may result in multiple web service requests, which may affect rate
    /// limiting.
    /// </remarks>
    public async Task ImportListensAsync(params ISubmittedListen[] listens) {
      var payload = SubmissionPayload.CreateImport();
      payload.Listens.AddRange(listens);
      foreach (var json in this.SerializeImport(payload)) {
        var task = this.PerformRequestAsync("submit-listens", Method.Post, json);
        await task.ConfigureAwait(false);
      }
    }

    private IEnumerable<string> SerializeImport(SubmissionPayload<ISubmittedListen> payload) {
      var json = JsonSerializer.Serialize(payload, ListenBrainz.JsonOptionsForWrite);
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
    public void SetNowPlaying(ISubmittedListenData listen) {
      var payload = SubmissionPayload.CreatePlayingNow(listen);
      var json = JsonSerializer.Serialize(payload, ListenBrainz.JsonOptionsForWrite);
      this.PerformRequest("submit-listens", Method.Post, json);
    }

    /// <summary>Sets the "now playing" information for the user whose token is set in <see cref="UserToken"/>.</summary>
    /// <param name="track">The name of the track being listened to.</param>
    /// <param name="artist">The name of the artist performing the track being listened to.</param>
    /// <param name="release">The name of the release containing the track being listened to.</param>
    /// <remarks>
    /// This will access the <c>POST /1/submit-listens</c> endpoint.<br/>
    /// Users can find their token on their profile page:
    /// <a href="https://listenbrainz.org/profile/">https://listenbrainz.org/profile/</a>.
    /// </remarks>
    public void SetNowPlaying(string track, string artist, string? release = null) {
      var listen = new SubmittedListenData(track, artist);
      listen.Track.Release = release;
      this.SetNowPlaying(listen);
    }

    /// <summary>Sets the "now playing" information for the user whose token is set in <see cref="UserToken"/>.</summary>
    /// <param name="listen">The listen data to send.</param>
    /// <return>A task that will perform the operation.</return>
    /// <remarks>
    /// This will access the <c>POST /1/submit-listens</c> endpoint.<br/>
    /// Users can find their token on their profile page:
    /// <a href="https://listenbrainz.org/profile/">https://listenbrainz.org/profile/</a>.
    /// </remarks>
    public async Task SetNowPlayingAsync(ISubmittedListenData listen) {
      var payload = SubmissionPayload.CreatePlayingNow(listen);
      var json = JsonSerializer.Serialize(payload, ListenBrainz.JsonOptionsForWrite);
      var task = this.PerformRequestAsync("submit-listens", Method.Post, json);
      await task.ConfigureAwait(false);
    }

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
    public async Task SetNowPlayingAsync(string track, string artist, string? release = null) {
      var listen = new SubmittedListenData(track, artist);
      listen.Track.Release = release;
      await this.SetNowPlayingAsync(listen);
    }

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
    public void SubmitSingleListen(ISubmittedListen listen) {
      var payload = SubmissionPayload.CreateSingle(listen);
      var json = JsonSerializer.Serialize(payload, ListenBrainz.JsonOptionsForWrite);
      this.PerformRequest("submit-listens", Method.Post, json);
    }

    /// <summary>
    /// Submits a single listen (typically one that has just completed) for the user whose token is set in <see cref="UserToken"/>.
    /// </summary>
    /// <param name="track">The name of the track being listened to.</param>
    /// <param name="artist">The name of the artist performing the track being listened to.</param>
    /// <param name="timestamp">
    /// The date and time at which the track was listened to; when not specified or <see langword="null"/>, the current UTC date and
    /// time is used.
    /// </param>
    /// <param name="release">The name of the release containing the track being listened to.</param>
    /// <remarks>
    /// This will access the <c>POST /1/submit-listens</c> endpoint.<br/>
    /// Users can find their token on their profile page:
    /// <a href="https://listenbrainz.org/profile/">https://listenbrainz.org/profile/</a>.
    /// </remarks>
    public void SubmitSingleListen(string track, string artist, DateTimeOffset? timestamp = null, string? release = null) {
      var listen = new SubmittedListen(track, artist, timestamp);
      listen.Track.Release = release;
      this.SubmitSingleListen(listen);
    }

    /// <summary>
    /// Submits a single listen (typically one that has just completed) for the user whose token is set in <see cref="UserToken"/>.
    /// </summary>
    /// <param name="track">The name of the track being listened to.</param>
    /// <param name="artist">The name of the artist performing the track being listened to.</param>
    /// <param name="timestamp">
    /// The date and time at which the track was listened to, expressed as the number of seconds since
    /// <see cref="UnixTime.Epoch">the Unix time epoch</see>.
    /// </param>
    /// <param name="release">The name of the release containing the track being listened to.</param>
    /// <remarks>
    /// This will access the <c>POST /1/submit-listens</c> endpoint.<br/>
    /// Users can find their token on their profile page:
    /// <a href="https://listenbrainz.org/profile/">https://listenbrainz.org/profile/</a>.
    /// </remarks>
    public void SubmitSingleListen(string track, string artist, long timestamp, string? release = null) {
      var listen = new SubmittedListen(track, artist, timestamp);
      listen.Track.Release = release;
      this.SubmitSingleListen(listen);
    }

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
    public async Task SubmitSingleListenAsync(ISubmittedListen listen) {
      var payload = SubmissionPayload.CreateSingle(listen);
      var json = JsonSerializer.Serialize(payload, ListenBrainz.JsonOptionsForWrite);
      var task = this.PerformRequestAsync("submit-listens", Method.Post, json);
      await task.ConfigureAwait(false);
    }

    /// <summary>
    /// Submits a single listen (typically one that has just completed) for the user whose token is set in <see cref="UserToken"/>.
    /// </summary>
    /// <param name="track">The name of the track being listened to.</param>
    /// <param name="artist">The name of the artist performing the track being listened to.</param>
    /// <param name="timestamp">
    /// The date and time at which the track was listened to; when not specified or <see langword="null"/>, the current UTC date and
    /// time is used.
    /// </param>
    /// <param name="release">The name of the release containing the track being listened to.</param>
    /// <return>A task that will perform the operation.</return>
    /// <remarks>
    /// This will access the <c>POST /1/submit-listens</c> endpoint.<br/>
    /// Users can find their token on their profile page:
    /// <a href="https://listenbrainz.org/profile/">https://listenbrainz.org/profile/</a>.
    /// </remarks>
    public async Task SubmitSingleListenAsync(string track, string artist, DateTimeOffset? timestamp = null, string? release = null) {
      var listen = new SubmittedListen(track, artist, timestamp);
      listen.Track.Release = release;
      await this.SubmitSingleListenAsync(listen);
    }

    /// <summary>
    /// Submits a single listen (typically one that has just completed) for the user whose token is set in <see cref="UserToken"/>.
    /// </summary>
    /// <param name="track">The name of the track being listened to.</param>
    /// <param name="artist">The name of the artist performing the track being listened to.</param>
    /// <param name="timestamp">
    /// The date and time at which the track was listened to, expressed as the number of seconds since
    /// <see cref="UnixTime.Epoch">the Unix time epoch</see>.
    /// </param>
    /// <param name="release">The name of the release containing the track being listened to.</param>
    /// <return>A task that will perform the operation.</return>
    /// <remarks>
    /// This will access the <c>POST /1/submit-listens</c> endpoint.<br/>
    /// Users can find their token on their profile page:
    /// <a href="https://listenbrainz.org/profile/">https://listenbrainz.org/profile/</a>.
    /// </remarks>
    public async Task SubmitSingleListenAsync(string track, string artist, long timestamp, string? release = null) {
      var listen = new SubmittedListen(track, artist, timestamp);
      listen.Track.Release = release;
      await this.SubmitSingleListenAsync(listen);
    }

    #endregion

    #endregion

    #region /1/user/xxx/listens

    private static NameValueCollection OptionsForGetListens(int? count, long? after, long? before) {
      var options = new NameValueCollection(3);
      if (before.HasValue)
        options.Set("max_ts", before.Value.ToString(CultureInfo.InvariantCulture));
      if (after.HasValue)
        options.Set("min_ts", after.Value.ToString(CultureInfo.InvariantCulture));
      if (count.HasValue)
        options.Set("count", count.Value.ToString(CultureInfo.InvariantCulture));
      return options;
    }

    /// <summary>Gets the most recent listens for a user.</summary>
    /// <param name="user">The MusicBrainz ID of the user whose data is needed.</param>
    /// <param name="count">
    /// The number of listens to return; must be no greater than <see cref="MaxItemsPerGet"/>.<br/>
    /// If not specified, this will return up to <see cref="DefaultItemsPerGet"/> listens.
    /// </param>
    /// <returns>The requested listens.</returns>
    public IFetchedListens? GetListens(string user, int? count = null)
      => this.GetListens(user, (long?) null, null, count);

    /// <summary>Gets the most recent listens for a user.</summary>
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
    /// The number of listens to return; must be no greater than <see cref="MaxItemsPerGet"/>.<br/>
    /// If not specified, this will return <see cref="DefaultItemsPerGet"/> listens
    /// </param>
    /// <returns>The requested listens.</returns>
    public IFetchedListens? GetListens(string user, long? after, long? before, int? count = null) {
      var options = ListenBrainz.OptionsForGetListens(count, after, before);
      var json = this.PerformRequest($"user/{user}/listens", Method.Get, options);
      return JsonUtils.Deserialize<Payload<FetchedListens>>(json, ListenBrainz.JsonOptionsForRead).Contents;
    }

    /// <summary>Gets the most recent listens for a user.</summary>
    /// <param name="user">The MusicBrainz ID of the user whose data is needed.</param>
    /// <param name="after">
    /// The timestamp to start from. Returned listens will have a timestamp greater than, but not including, this value.
    /// </param>
    /// <param name="before">
    /// The timestamp to end at. Returned listens will have a timestamp less than, but not including, this value.
    /// </param>
    /// <param name="count">
    /// The number of listens to return; must be no greater than <see cref="MaxItemsPerGet"/>.<br/>
    /// If not specified, this will return <see cref="DefaultItemsPerGet"/> listens
    /// </param>
    /// <returns>The requested listens.</returns>
    public IFetchedListens? GetListens(string user, DateTimeOffset? after, DateTimeOffset? before, int? count = null)
      => this.GetListens(user, UnixTime.Convert(after), UnixTime.Convert(before), count);

    /// <summary>Gets the most recent listens for a user.</summary>
    /// <param name="user">The MusicBrainz ID of the user whose data is needed.</param>
    /// <param name="count">
    /// The number of listens to return; must be no greater than <see cref="MaxItemsPerGet"/>.<br/>
    /// If not specified, this will return up to <see cref="DefaultItemsPerGet"/> listens.
    /// </param>
    /// <returns>A task returning the requested listens.</returns>
    public Task<IFetchedListens?> GetListensAsync(string user, int? count = null)
      => this.GetListensAsync(user, (long?) null, null, count);

    /// <summary>Gets the most recent listens for a user.</summary>
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
    /// The number of listens to return; must be no greater than <see cref="MaxItemsPerGet"/>.<br/>
    /// If not specified, this will return <see cref="DefaultItemsPerGet"/> listens.
    /// </param>
    /// <returns>A task returning the requested listens.</returns>
    public async Task<IFetchedListens?> GetListensAsync(string user, long? after, long? before, int? count = null) {
      var options = ListenBrainz.OptionsForGetListens(count, after, before);
      var json = await this.PerformRequestAsync($"user/{user}/listens", Method.Get, options);
      return JsonUtils.Deserialize<Payload<FetchedListens>>(json, ListenBrainz.JsonOptionsForRead).Contents;
    }

    /// <summary>Gets the most recent listens for a user.</summary>
    /// <param name="user">The MusicBrainz ID of the user whose data is needed.</param>
    /// <param name="after">
    ///   The timestamp to start from. Returned listens will have a timestamp greater than, but not including, this value.
    /// </param>
    /// <param name="before">
    ///   The timestamp to end at. Returned listens will have a timestamp less than, but not including, this value.
    /// </param>
    /// <param name="count">
    ///   The number of listens to return; must be no greater than <see cref="MaxItemsPerGet"/>.<br/>
    ///   If not specified, this will return <see cref="DefaultItemsPerGet"/> listens.
    /// </param>
    /// <returns>A task returning the requested listens.</returns>
    public Task<IFetchedListens?> GetListensAsync(string user, DateTimeOffset? after, DateTimeOffset? before, int? count = null)
      => this.GetListensAsync(user, UnixTime.Convert(after), UnixTime.Convert(before), count);

    #endregion

    #region /1/user/xxx/playing-now

    /// <summary>Gets a user's currently-playing listen(s).</summary>
    /// <param name="user">The MusicBrainz ID of the user whose data is needed.</param>
    /// <returns>The requested listens (typically 0 or 1).</returns>
    public IFetchedListens? GetPlayingNow(string user) {
      var json = this.PerformRequest($"user/{user}/playing-now", Method.Get);
      return JsonUtils.Deserialize<Payload<FetchedListens>>(json, ListenBrainz.JsonOptionsForRead).Contents;
    }

    /// <summary>Gets a user's currently-playing listen(s).</summary>
    /// <param name="user">The MusicBrainz ID of the user whose data is needed.</param>
    /// <returns>A task returning the requested listens (typically 0 or 1).</returns>
    public async Task<IFetchedListens?> GetPlayingNowAsync(string user) {
      var json = await this.PerformRequestAsync($"user/{user}/playing-now", Method.Get);
      return JsonUtils.Deserialize<Payload<FetchedListens>>(json, ListenBrainz.JsonOptionsForRead).Contents;
    }

    #endregion

    #region /1/users/xxx/recent-listens

    /// <summary>Gets recent listen(s) for a set of users.</summary>
    /// <param name="users">The MusicBrainz IDs of the users whose data is needed.</param>
    /// <returns>The requested listens.</returns>
    public IFetchedListens? GetRecentListens(params string[] users) {
      var userList = string.Join(",", users.Select(Uri.EscapeDataString));
      var json = this.PerformRequest($"users/{userList}/recent-listens", Method.Get);
      return JsonUtils.Deserialize<Payload<FetchedListens>>(json, ListenBrainz.JsonOptionsForRead).Contents;
    }

    /// <summary>Gets recent listen(s) for a set of users.</summary>
    /// <param name="limit">The maximum number of listens to return.</param>
    /// <param name="users">The MusicBrainz IDs of the users whose data is needed.</param>
    /// <returns>The requested listens.</returns>
    public IFetchedListens? GetRecentListens(int limit, params string[] users) {
      var userList = string.Join(",", users.Select(Uri.EscapeDataString));
      var json = this.PerformRequest($"users/{userList}/recent-listens", Method.Get);
      return JsonUtils.Deserialize<Payload<FetchedListens>>(json, ListenBrainz.JsonOptionsForRead).Contents;
    }

    /// <summary>Gets recent listen(s) for a set of users.</summary>
    /// <param name="users">The MusicBrainz IDs of the users whose data is needed.</param>
    /// <returns>A task returning the requested listens.</returns>
    public async Task<IFetchedListens?> GetRecentListensAsync(params string[] users) {
      var userList = string.Join(",", users.Select(Uri.EscapeDataString));
      var json = await this.PerformRequestAsync($"users/{userList}/recent-listens", Method.Get);
      return JsonUtils.Deserialize<Payload<FetchedListens>>(json, ListenBrainz.JsonOptionsForRead).Contents;
    }

    /// <summary>Gets recent listen(s) for a set of users.</summary>
    /// <param name="limit">The maximum number of listens to return.</param>
    /// <param name="users">The MusicBrainz IDs of the users whose data is needed.</param>
    /// <returns>A task returning the requested listens.</returns>
    public async Task<IFetchedListens?> GetRecentListensAsync(int limit, params string[] users) {
      var userList = string.Join(",", users.Select(Uri.EscapeDataString));
      var json = await this.PerformRequestAsync($"users/{userList}/recent-listens", Method.Get);
      return JsonUtils.Deserialize<Payload<FetchedListens>>(json, ListenBrainz.JsonOptionsForRead).Contents;
    }

    #endregion

    #region /1/validate-token

    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    private sealed class TokenValidationResult {

      [JsonPropertyName("code")]
      public int? Code { get; set; }

      [JsonPropertyName("message")]
      public string? Message { get; set; }

      [JsonPropertyName("user_name")]
      public string? User { get; set; }

      [JsonPropertyName("valid")]
      public bool? Valid { get; set; }

    }

    /// <summary>Validates a given user token.</summary>
    /// <param name="token">The user token to validate.</param>
    /// <returns><see langword="true"/> when <paramref name="token"/> is valid; <see langword="false"/> otherwise.</returns>
    public bool ValidateToken(string token) => this.ValidateToken(token, out _);

    /// <summary>Validates a given user token.</summary>
    /// <param name="token">The user token to validate.</param>
    /// <param name="user">The name of the user associated with the user token, if available.</param>
    /// <returns><see langword="true"/> when <paramref name="token"/> is valid; <see langword="false"/> otherwise.</returns>
    public bool ValidateToken(string token, out string? user) {
      var json = this.PerformRequest($"validate-token?token={token}", Method.Get);
      var tvr = JsonUtils.Deserialize<TokenValidationResult>(json);
      user = tvr.User;
      if (tvr.Valid.HasValue)
        return tvr.Valid.Value;
      // Older implementations only return a message.
      return tvr.Code == 200 && tvr.Message == "Token valid.";
    }

    /// <summary>Validates a given user token.</summary>
    /// <param name="token">The user token to validate.</param>
    /// <returns>
    /// A task returning a tuple containing a flag indicating the validity of <paramref name="token"/> (<see langword="true"/> when
    /// it is valid; <see langword="false"/> otherwise) and the name of the user associated with the user token, if available.
    /// </returns>
    public async Task<(bool, string?)> ValidateTokenAsync(string token) {
      var json = await this.PerformRequestAsync($"validate-token?token={token}", Method.Get).ConfigureAwait(false);
      var tvr = JsonUtils.Deserialize<TokenValidationResult>(json);
      if (tvr.Valid.HasValue)
        return (tvr.Valid.Value, tvr.User);
      // Older implementations only return a message.
      return (tvr.Code == 200 && tvr.Message == "Token valid.", null);
    }

    #endregion

    #endregion

    #region Internals

    private static readonly JsonSerializerOptions JsonOptionsForRead = new JsonSerializerOptions {
      // @formatter:off
      AllowTrailingCommas         = false,
      IgnoreNullValues            = false,
      PropertyNameCaseInsensitive = false,
      // @formatter:on
      Converters = {
        // Mappers for interfaces that appear in scalar properties.
        // @formatter:off
        new InterfaceConverter<IAdditionalInfo, AdditionalInfo>(),
        new InterfaceConverter<ITrackInfo,      TrackInfo     >(),
        // @formatter:on
        // Mappers for interfaces that appear in array properties.
        // @formatter:off
        new ReadOnlyListOfInterfaceConverter<IListen, Listen>(),
        // @formatter:on
        // This one is for UnhandledProperties - it tries to create useful types for a field of type 'object'
        new AnyObjectConverter(),
      }
    };

    private static readonly JsonSerializerOptions JsonOptionsForWrite = new JsonSerializerOptions() {
      // @formatter:off
      IgnoreNullValues         = false,
      IgnoreReadOnlyProperties = false,
      // @formatter:on
#if DEBUG
      WriteIndented = true,
#else
      WriteIndented = false,
#endif
      Converters = { new SubmissionSerializer() }
    };

    #region Web Client / IDisposable

    private readonly SemaphoreSlim _clientLock = new SemaphoreSlim(1);

    private bool _disposed;

    private readonly string _fullUserAgent;

    private WebClient? _webClient;

    private WebClient WebClient {
      get {
        if (this._disposed)
          throw new ObjectDisposedException(nameof(ListenBrainz));
        var wc = this._webClient ??= new WebClient { Encoding = Encoding.UTF8 };
        wc.BaseAddress = this.BaseUri.ToString();
        return wc;
      }
    }

    /// <summary>Closes the web client in use by this query, if there is one.</summary>
    /// <remarks>The next web service request will create a new client.</remarks>
    public void Close() {
      this._clientLock.Wait();
      try {
        this._webClient?.Dispose();
        this._webClient = null;
      }
      finally {
        this._clientLock.Release();
      }
    }

    /// <summary>Disposes the web client in use by this query, if there is one.</summary>
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
        this._clientLock.Dispose();
      }
      finally {
        this._disposed = true;
      }
    }

    /// <summary>Finalizes this instance.</summary>
    ~ListenBrainz() {
      this.Dispose(false);
    }

    #endregion

    #region Basic Request Execution

    private WebClient PrepareRequest(NameValueCollection? options) {
      var wc = this.WebClient;
      wc.Headers.Set("Content-Type", "application/json");
      wc.Headers.Set("Accept",       "application/json");
      wc.Headers.Set("User-Agent",   this._fullUserAgent);
      if (this.UserToken != null)
        wc.Headers.Set("Authorization", "Token " + this.UserToken);
      wc.QueryString.Clear();
      if (options != null)
        wc.QueryString.Add(options);
      return wc;
    }

    private string PerformRequest(string address, Method method, NameValueCollection? options = null) {
      return this.PerformRequest(address, method, null, options);
    }

    private string PerformRequest(string address, Method method, string? body, NameValueCollection? options = null) {
      Debug.Print($"[{DateTime.UtcNow}] WEB SERVICE REQUEST: {method} {this.BaseUri}{address}");
      this._clientLock.Wait();
      try {
        var wc = this.PrepareRequest(options);
        string? response = null;
        try {
          if (method == Method.Get)
            return response = wc.DownloadString(address);
          else {
            if (body != null)
              Debug.Print($"[{DateTime.UtcNow}] => BODY: {body}");
            return response = wc.UploadString(address, method.ToString(), body ?? string.Empty);
          }
        }
        catch (WebException we) {
          var ei = ErrorInfo.ExtractFrom(we.Response as HttpWebResponse);
          if (ei != null)
            throw new QueryException(ei.Code, ei.Error, we);
          throw;
        }
        finally {
          if (response != null)
            Debug.Print($"[{DateTime.UtcNow}] => RESPONSE: {response}");
          this.RateLimitInfo = RateLimitInfo.From(wc.ResponseHeaders);
        }
      }
      finally {
        this._clientLock.Release();
      }
    }

    private Task<string> PerformRequestAsync(string address, Method method, NameValueCollection? options = null) {
      return this.PerformRequestAsync(address, method, null, options);
    }

    private async Task<string> PerformRequestAsync(string address, Method method, string? body, NameValueCollection? options = null) {
      Debug.Print($"[{DateTime.UtcNow}] WEB SERVICE REQUEST: {method} {this.BaseUri}{address}");
      await this._clientLock.WaitAsync();
      try {
        var wc = this.PrepareRequest(options);
        string? response = null;
        try {
          if (method == Method.Get)
            return response = await wc.DownloadStringTaskAsync(address).ConfigureAwait(false);
          else {
            if (body != null)
              Debug.Print($"[{DateTime.UtcNow}] => BODY: {body}");
            return response = await wc.UploadStringTaskAsync(address, method.ToString(), body ?? string.Empty).ConfigureAwait(false);
          }
        }
        catch (WebException we) {
          var ei = await ErrorInfo.ExtractFromAsync(we.Response as HttpWebResponse);
          if (ei != null)
            throw new QueryException(ei.Code, ei.Error, we);
          throw;
        }
        finally {
          if (response != null)
            Debug.Print($"[{DateTime.UtcNow}] => RESPONSE: {response}");
          this.RateLimitInfo = RateLimitInfo.From(wc.ResponseHeaders);
        }
      }
      finally {
        this._clientLock.Release();
      }
    }

    #endregion

    #endregion

  }

}
