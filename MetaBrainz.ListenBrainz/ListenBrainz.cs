using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
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

    /// <summary>The user token to use for authenticated requests.</summary>
    /// <remarks>
    /// A user can find their token on their profile page (e.g.
    /// <a href="https://listenbrainz.org/profile/">https://listenbrainz.org/profile/</a>).
    /// </remarks>
    public string UserToken { get; set; } = null;

    #endregion

    #region Public API

    /// <summary>Information about the active rate limiting. Gets refreshed after every API call.</summary>
    public RateLimitInfo RateLimitInfo { get; private set; }

    #region /1/latest-import

    /// <summary>Get the timestamp of the newest listen submitted by a user in previous imports to ListenBrainz.</summary>
    /// <param name="userName">The MusicBrainz ID of the user whose data is needed.</param>
    /// <returns>An object providing the user's ID and latest import timestamp.</returns>
    /// <remarks>This will access the <c>GET /1/latest-import</c> endpoint.</remarks>
    public ILatestImport GetLatestImport(string userName) {
      var json = this.PerformRequest("latest-import", Method.GET, userName);
      return JsonConvert.DeserializeObject<LatestImport>(json);
    }

    /// <summary>Get the timestamp of the newest listen submitted by a user in previous imports to ListenBrainz.</summary>
    /// <param name="userName">The MusicBrainz ID of the user whose data is needed.</param>
    /// <returns>An object providing the user's ID and latest import timestamp.</returns>
    /// <remarks>This will access the <c>GET /1/latest-import</c> endpoint.</remarks>
    public async Task<ILatestImport> GetLatestImportAsync(string userName) {
      var json = await this.PerformRequestAsync("latest-import", Method.GET, userName).ConfigureAwait(false);
      return JsonConvert.DeserializeObject<LatestImport>(json);
    }

    #endregion

    #endregion

    #region Internals

    private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings {
      CheckAdditionalContent = true,
      MissingMemberHandling  = MissingMemberHandling.Error
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

    private string PerformRequest(string address, Method method, string userName = null, string body = null) {
      Debug.Print($"[{DateTime.UtcNow}] WEB SERVICE REQUEST: {method} {this.BaseUri}{address}");
      this._clientLock.Wait();
      try {
        var wc = this.WebClient;
        wc.Headers.Set("Content-Type", "application/json");
        wc.Headers.Set("Accept",       "application/json");
        wc.Headers.Set("User-Agent",   this._fullUserAgent);
        if (this.UserToken != null)
          wc.Headers.Set("Authorization", this.UserToken);
        wc.QueryString.Clear();
        if (userName != null)
          wc.QueryString.Set("user_name", userName);
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
        // TODO: Maybe catch any WebException from the requests and map to a nicer error?
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

    private async Task<string> PerformRequestAsync(string address, Method method, string userName, string body = null) {
      Debug.Print($"[{DateTime.UtcNow}] WEB SERVICE REQUEST: {method} {this.BaseUri}{address}");
      await this._clientLock.WaitAsync();
      try {
        var wc = this.WebClient;
        wc.Headers.Set("Content-Type", "application/json");
        wc.Headers.Set("Accept",       "application/json");
        wc.Headers.Set("User-Agent",   this._fullUserAgent);
        if (this.UserToken != null)
          wc.Headers.Set("Authorization", this.UserToken);
        wc.QueryString.Clear();
        if (userName != null)
          wc.QueryString.Set("user_name", userName);
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
        // TODO: Maybe catch any WebException from the requests and map to a nicer error?
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
