using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;

using JetBrains.Annotations;

using MetaBrainz.Common;

namespace MetaBrainz.ListenBrainz;

/// <summary>Main class for accessing the ListenBrainz API.</summary>
[PublicAPI]
public sealed partial class ListenBrainz : IDisposable {

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

}
