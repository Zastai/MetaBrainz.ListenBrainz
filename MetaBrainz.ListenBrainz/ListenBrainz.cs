using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using MetaBrainz.ListenBrainz.Interfaces;
using MetaBrainz.ListenBrainz.Objects;

using Newtonsoft.Json;

namespace MetaBrainz.ListenBrainz {

  /// <summary>Main class for accessing the ListenBrainz API.</summary>
  [SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
  [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
  [SuppressMessage("ReSharper", "UnusedMember.Global")]
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
    public static string DefaultUserAgent { get; set; } = null;

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
    public ListenBrainz(string userAgent = null) {
      this.UserAgent = userAgent ?? ListenBrainz.DefaultUserAgent;
      if (this.UserAgent == null) throw new ArgumentNullException(nameof(userAgent));
      if (this.UserAgent.Trim().Length == 0) throw new ArgumentException("The user agent must not be blank.", nameof(userAgent));
      { // Set full user agent, including this library's information
        var an = typeof(ListenBrainz).Assembly.GetName();
        this._fullUserAgent = $"{this.UserAgent} {an.Name}/{an.Version} ({ListenBrainz.UserAgentUrl})";
      }
    }

    /// <summary>Creates a new instance of the <see cref="T:ListenBrainz" /> class.</summary>
    /// <param name="application">The application name to use in the user agent property for all requests.</param>
    /// <param name="version">The version number to use in the user agent property for all requests.</param>
    /// <param name="contact">
    /// The contact address (typically HTTP, HTTPS or MAILTO) to use in the user agent property for all requests.
    /// </param>
    /// <exception cref="T:System.ArgumentException">When <paramref name="application"/> is blank.</exception>
    /// <exception cref="T:System.ArgumentNullException">
    /// When <paramref name="application"/>, <paramref name="version"/> and/or <paramref name="contact"/> are
    /// <see langword="null"/>.
    /// </exception>
    public ListenBrainz(string application, Version version, Uri contact)
    : this(application, version?.ToString(), contact?.ToString())
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
    /// <exception cref="ArgumentNullException">
    /// When <paramref name="application"/>, <paramref name="version"/> and/or <paramref name="contact"/> are
    /// <see langword="null"/>.
    /// </exception>
    public ListenBrainz(string application, string version, string contact) {
      if (application == null) throw new ArgumentNullException(nameof(application));
      if (version     == null) throw new ArgumentNullException(nameof(version));
      if (contact     == null) throw new ArgumentNullException(nameof(contact));
      if (application.Trim().Length == 0) throw new ArgumentException("The application name must not be blank.", nameof(application));
      if (version    .Trim().Length == 0) throw new ArgumentException("The version number must not be blank.",   nameof(version));
      if (contact    .Trim().Length == 0) throw new ArgumentException("The contact address must not be blank.",  nameof(contact));
      this.UserAgent = $"{application}/{version} ({contact})";
      { // Set full user agent, including this library's information
        var an = typeof(ListenBrainz).Assembly.GetName();
        this._fullUserAgent = $"{this.UserAgent} {an.Name}/{an.Version} ({ListenBrainz.UserAgentUrl})";
      }
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

    #endregion

    #region Public API

    /// <summary>Information about the active rate limiting. Gets refreshed after every API call.</summary>
    public RateLimitInfo RateLimitInfo { get; private set; }

    #region /1/latest-import

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    private static NameValueCollection OptionsForLatestImport(string user_name) {
      var options = new NameValueCollection(1);
      options.Set("user_name", user_name);
      return options;
    }

    /// <summary>Get the timestamp of the newest listen submitted by a user in previous imports to ListenBrainz.</summary>
    /// <param name="user">The MusicBrainz ID of the user whose data is requested.</param>
    /// <returns>An object providing the user's ID and latest import timestamp.</returns>
    /// <remarks>This will access the <c>GET /1/latest-import</c> endpoint.</remarks>
    public ILatestImport GetLatestImport(string user) {
      var json = this.PerformRequest("latest-import", Method.GET, ListenBrainz.OptionsForLatestImport(user));
      return JsonConvert.DeserializeObject<LatestImport>(json);
    }

    /// <summary>Get the timestamp of the newest listen submitted by a user in previous imports to ListenBrainz.</summary>
    /// <param name="user">The MusicBrainz ID of the user whose data is requested.</param>
    /// <returns>An object providing the user's ID and latest import timestamp.</returns>
    /// <remarks>This will access the <c>GET /1/latest-import</c> endpoint.</remarks>
    public async Task<ILatestImport> GetLatestImportAsync(string user) {
      var task = this.PerformRequestAsync("latest-import", Method.GET, ListenBrainz.OptionsForLatestImport(user));
      var json = await task.ConfigureAwait(false);
      return JsonConvert.DeserializeObject<LatestImport>(json);
    }

    /// <summary>Set the timestamp of the newest listen submitted by a user in previous imports to ListenBrainz.</summary>
    /// <param name="user">The MusicBrainz ID of the user whose data should be modified.</param>
    /// <param name="token">The user's authorization token.</param>
    /// <param name="timestamp">The timestamp to set.</param>
    /// <remarks>This will access the <c>POST /1/latest-import</c> endpoint.</remarks>
    /// <remarks>
    /// Users can find their token on their profile page:
    /// <a href="https://listenbrainz.org/profile/">https://listenbrainz.org/profile/</a>.
    /// </remarks>
    public void SetLatestImport(string user, string token, DateTime timestamp) {
      this.SetLatestImport(user, token, UnixTime.Convert(timestamp));
    }

    /// <summary>Set the timestamp of the newest listen submitted by a user in previous imports to ListenBrainz.</summary>
    /// <param name="user">The MusicBrainz ID of the user whose data is needed.</param>
    /// <param name="token">The user's authorization token.</param>
    /// <param name="timestamp">
    /// The timestamp to set, specified as the number of seconds since <see cref="UnixTime.Epoch">the Unix time epoch</see>.
    /// </param>
    /// <remarks>This will access the <c>POST /1/latest-import</c> endpoint.</remarks>
    /// <remarks>
    /// Users can find their token on their profile page:
    /// <a href="https://listenbrainz.org/profile/">https://listenbrainz.org/profile/</a>.
    /// </remarks>
    public void SetLatestImport(string user, string token, long timestamp) {
      this.PerformRequest("latest-import", Method.POST, $"{{ ts: {timestamp} }}", OptionsForLatestImport(user), token);
    }

    /// <summary>Set the timestamp of the newest listen submitted by a user in previous imports to ListenBrainz.</summary>
    /// <param name="user">The MusicBrainz ID of the user whose data should be modified.</param>
    /// <param name="token">The user's authorization token.</param>
    /// <param name="timestamp">The timestamp to set.</param>
    /// <remarks>This will access the <c>POST /1/latest-import</c> endpoint.</remarks>
    /// <remarks>
    /// Users can find their token on their profile page:
    /// <a href="https://listenbrainz.org/profile/">https://listenbrainz.org/profile/</a>.
    /// </remarks>
    public Task SetLatestImportAsync(string user, string token, DateTime timestamp) {
      return this.SetLatestImportAsync(user, token, UnixTime.Convert(timestamp));
    }

    /// <summary>Set the timestamp of the newest listen submitted by a user in previous imports to ListenBrainz.</summary>
    /// <param name="user">The MusicBrainz ID of the user whose data is needed.</param>
    /// <param name="token">The user's authorization token.</param>
    /// <param name="timestamp">
    /// The timestamp to set, specified as the number of seconds since <see cref="UnixTime.Epoch">the Unix time epoch</see>.
    /// </param>
    /// <remarks>This will access the <c>POST /1/latest-import</c> endpoint.</remarks>
    /// <remarks>
    /// Users can find their token on their profile page:
    /// <a href="https://listenbrainz.org/profile/">https://listenbrainz.org/profile/</a>.
    /// </remarks>
    public async Task SetLatestImportAsync(string user, string token, long timestamp) {
      var task = this.PerformRequestAsync("latest-import", Method.POST, $"{{ ts: {timestamp} }}", OptionsForLatestImport(user), token);
      await task.ConfigureAwait(false);
    }

    #endregion

    #region /1/refresh-spotify-token

    // TODO

    #endregion

    #region /1/submit-listens

    // TODO

    #endregion

    #region /1/user/xxx/listens

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    private static NameValueCollection OptionsForGetListens(int? count, long? min_ts, long? max_ts) {
      var options = new NameValueCollection(3);
      if (max_ts.HasValue)
        options.Set("max_ts", max_ts.Value.ToString(CultureInfo.InvariantCulture));
      if (min_ts.HasValue)
        options.Set("min_ts", min_ts.Value.ToString(CultureInfo.InvariantCulture));
      if (count.HasValue)
        options.Set("count", count.Value.ToString(CultureInfo.InvariantCulture));
      return options;
    }

    /// <summary>Gets the most recent listens for a user.</summary>
    /// <param name="user">The MusicBrainz ID of the user whose data is needed.</param>
    /// <param name="token">
    /// The user token to use for the request.<br/>
    /// While this must be a <em>valid</em> user token, it does not need to be that of <paramref name="user"/>.<br/>
    /// Passing <see langword="null"/> is valid, but will be subject to stricter rate limiting.
    /// </param>
    /// <param name="count">
    /// The number of listens to return; must be no greater than <see cref="MaxItemsPerGet"/>.<br/>
    /// If not specified, this will return <see cref="DefaultItemsPerGet"/> listens
    /// </param>
    /// <returns>The requested listens.</returns>
    public IFetchedListens GetListens(string user, string token = null, int? count = null) {
      var json = this.PerformRequest("user/" + user + "/listens", Method.GET, OptionsForGetListens(count, null, null));
      return JsonConvert.DeserializeObject<Payload<FetchedListens>>(json, ListenBrainz.SerializerSettings).Contents;
    }

    /// <summary>Gets listens for a user, starting from a particular timestamp.</summary>
    /// <param name="user">The MusicBrainz ID of the user whose data is needed.</param>
    /// <param name="timestamp">
    /// The timestamp to start from.
    /// Returned listens will have a timestamp greater than, but not including, this value.
    /// </param>
    /// <param name="token">
    /// The user token to use for the request.<br/>
    /// While this must be a <em>valid</em> user token, it does not need to be that of <paramref name="user"/>.<br/>
    /// Passing <see langword="null"/> is valid, but will be subject to stricter rate limiting.
    /// </param>
    /// <param name="count">
    /// The number of listens to return; must be no greater than <see cref="MaxItemsPerGet"/>.<br/>
    /// If not specified, this will return <see cref="DefaultItemsPerGet"/> listens
    /// </param>
    /// <returns>The requested listens.</returns>
    public IFetchedListens GetListensAfter(string user, DateTime timestamp, string token = null, int? count = null) {
      return this.GetListensAfter(user, UnixTime.Convert(timestamp), token, count);
    }

    /// <summary>Gets listens for a user, starting from a particular timestamp.</summary>
    /// <param name="user">The MusicBrainz ID of the user whose data is needed.</param>
    /// <param name="timestamp">
    /// The timestamp to start from, specified as the number of seconds since <see cref="UnixTime.Epoch">the Unix time epoch</see>.
    /// Returned listens will have a timestamp greater than, but not including, this value.
    /// </param>
    /// <param name="token">
    /// The user token to use for the request.<br/>
    /// While this must be a <em>valid</em> user token, it does not need to be that of <paramref name="user"/>.<br/>
    /// Passing <see langword="null"/> is valid, but will be subject to stricter rate limiting.
    /// </param>
    /// <param name="count">
    /// The number of listens to return; must be no greater than <see cref="MaxItemsPerGet"/>.<br/>
    /// If not specified, this will return <see cref="DefaultItemsPerGet"/> listens
    /// </param>
    /// <returns>The requested listens.</returns>
    public IFetchedListens GetListensAfter(string user, long timestamp, string token = null, int? count = null) {
      var json = this.PerformRequest("user/" + user + "/listens", Method.GET, OptionsForGetListens(count, timestamp, null), token);
      return JsonConvert.DeserializeObject<Payload<FetchedListens>>(json, ListenBrainz.SerializerSettings).Contents;
    }

    /// <summary>Gets listens for a user, ending at a particular timestamp.</summary>
    /// <param name="user">The MusicBrainz ID of the user whose data is needed.</param>
    /// <param name="timestamp">
    /// The timestamp to end at.
    /// Returned listens will have a timestamp less than, but not including, this value.
    /// </param>
    /// <param name="token">
    /// The user token to use for the request.<br/>
    /// While this must be a <em>valid</em> user token, it does not need to be that of <paramref name="user"/>.<br/>
    /// Passing <see langword="null"/> is valid, but will be subject to stricter rate limiting.
    /// </param>
    /// <param name="count">
    /// The number of listens to return; must be no greater than <see cref="MaxItemsPerGet"/>.<br/>
    /// If not specified, this will return <see cref="DefaultItemsPerGet"/> listens
    /// </param>
    /// <returns>The requested listens.</returns>
    public IFetchedListens GetListensBefore(string user, DateTime timestamp, string token = null, int? count = null) {
      return this.GetListensBefore(user, UnixTime.Convert(timestamp), token, count);
    }

    /// <summary>Gets listens for a user, ending at a particular timestamp.</summary>
    /// <param name="user">The MusicBrainz ID of the user whose data is needed.</param>
    /// <param name="timestamp">
    /// The timestamp to end at, specified as the number of seconds since <see cref="UnixTime.Epoch">the Unix time epoch</see>.
    /// Returned listens will have a timestamp less than, but not including, this value.
    /// </param>
    /// <param name="token">
    /// The user token to use for the request.<br/>
    /// While this must be a <em>valid</em> user token, it does not need to be that of <paramref name="user"/>.<br/>
    /// Passing <see langword="null"/> is valid, but will be subject to stricter rate limiting.
    /// </param>
    /// <param name="count">
    /// The number of listens to return; must be no greater than <see cref="MaxItemsPerGet"/>.<br/>
    /// If not specified, this will return <see cref="DefaultItemsPerGet"/> listens
    /// </param>
    /// <returns>The requested listens.</returns>
    public IFetchedListens GetListensBefore(string user, long timestamp, string token = null, int? count = null) {
      var json = this.PerformRequest("user/" + user + "/listens", Method.GET, OptionsForGetListens(count, null, timestamp), token);
      return JsonConvert.DeserializeObject<Payload<FetchedListens>>(json, ListenBrainz.SerializerSettings).Contents;
    }

    #endregion

    #region /1/user/xxx/playing-now

    // TODO

    #endregion

    #region /1/users/xxx/recent-listens

    // TODO

    #endregion

    #region /1/validate-token

    // TODO

    #endregion

    #endregion

    #region Internals

    private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings {
      CheckAdditionalContent = true,
      MissingMemberHandling  = MissingMemberHandling.Ignore
    };

    #region Web Client / IDisposable

    private readonly SemaphoreSlim _clientLock = new SemaphoreSlim(1);

    private bool _disposed;

    private readonly string _fullUserAgent;

    private WebClient _webClient;

    private WebClient WebClient {
      get {
        if (this._disposed)
          throw new ObjectDisposedException(nameof(ListenBrainz));
        var wc = this._webClient ?? (this._webClient = new WebClient { Encoding = Encoding.UTF8 });
        wc.BaseAddress = this.BaseUri.ToString();
        return wc;
      }
    }

    /// <summary>Closes the web client in use by this query, if there is one.</summary>
    /// <remarks>The next web service request will create a new client.</remarks>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
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

    #region Error Response Handling

    #pragma warning disable 649

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    private sealed class ErrorInfo {
      [JsonProperty] public int    code;
      [JsonProperty] public string error;
    }

    #pragma warning restore 649

    private static ErrorInfo ExtractError(WebResponse response) {
      if (response == null || response.ContentLength == 0)
        return null;
      try {
        var stream = response.GetResponseStream();
        if (stream == null || !stream.CanRead)
          return null;
        if (response.ContentType.StartsWith("application/json")) {
          var charset = (response as HttpWebResponse)?.CharacterSet;
          var encoding = string.IsNullOrWhiteSpace(charset) ? Encoding.UTF8 : Encoding.GetEncoding(charset);
          try {
            using (var sr = new StreamReader(stream, encoding, false, 1024, true)) {
              var json = sr.ReadToEnd();
              Debug.Print($"[{DateTime.UtcNow}] => RESPONSE ({response.ContentType}): \"{json}\"");
              var ei = JsonConvert.DeserializeObject<ErrorInfo>(json);
              Debug.Print($"[{DateTime.UtcNow}] => ERROR: ({ei?.code}): \"{ei?.error}\"");
              return ei;
            }
          }
          finally {
            if (stream.CanSeek)
              stream.Seek(0, SeekOrigin.Begin);
          }
        }
        Debug.Print($"[{DateTime.UtcNow}] => UNHANDLED ERROR RESPONSE ({response.ContentType}): <{response.ContentLength} byte(s)>");
      }
      catch (Exception e) {
        Debug.Print($"[{DateTime.UtcNow}] => FAILED TO PROCESS ERROR RESPONSE: [{e.GetType()}] {e.Message}");
        /* keep calm and fall through */
      }
      return null;
    }

    #endregion

    private WebClient PrepareRequest(NameValueCollection options, string token)
    {
      var wc = this.WebClient;
      wc.Headers.Set("Content-Type", "application/json");
      wc.Headers.Set("Accept",       "application/json");
      wc.Headers.Set("User-Agent",   this._fullUserAgent);
      if (token != null)
        wc.Headers.Set("Authorization", "Token " + token);
      wc.QueryString.Clear();
      if (options != null)
        wc.QueryString.Add(options);
      return wc;
    }

    private string PerformRequest(string address, Method method, NameValueCollection options = null, string token = null) {
      return this.PerformRequest(address, method, null, options, token);
    }

    private string PerformRequest(string address, Method method, string body, NameValueCollection options = null, string token = null) {
      Debug.Print($"[{DateTime.UtcNow}] WEB SERVICE REQUEST: {method} {this.BaseUri}{address}");
      this._clientLock.Wait();
      try {
        var wc = this.PrepareRequest(options, token);
        string response = null;
        try {
          if (method == Method.GET)
            return response = wc.DownloadString(address);
          else {
            if (body != null)
              Debug.Print($"[{DateTime.UtcNow}] => BODY: {body}");
            return response = wc.UploadString(address, method.ToString(), body ?? string.Empty);
          }
        }
        catch (WebException we) {
          var ei = ListenBrainz.ExtractError(we.Response);
          if (ei != null)
            throw new QueryException(ei.code, ei.error, we);
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

    private Task<string> PerformRequestAsync(string address, Method method, NameValueCollection options = null, string token = null) {
      return this.PerformRequestAsync(address, method, null, options, token);
    }

    private async Task<string> PerformRequestAsync(string address, Method method, string body, NameValueCollection options, string token = null) {
      Debug.Print($"[{DateTime.UtcNow}] WEB SERVICE REQUEST: {method} {this.BaseUri}{address}");
      await this._clientLock.WaitAsync();
      try {
        var wc = this.PrepareRequest(options, token);
        string response = null;
        try {
          if (method == Method.GET)
            return response = await wc.DownloadStringTaskAsync(address).ConfigureAwait(false);
          else {
            if (body != null)
              Debug.Print($"[{DateTime.UtcNow}] => BODY: {body}");
            return response = await wc.UploadStringTaskAsync(address, method.ToString(), body ?? string.Empty).ConfigureAwait(false);
          }
        }
        catch (WebException we) {
          var ei = ListenBrainz.ExtractError(we.Response);
          if (ei != null)
            throw new QueryException(ei.code, ei.error, we);
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
