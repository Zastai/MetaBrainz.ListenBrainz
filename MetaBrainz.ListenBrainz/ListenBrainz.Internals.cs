using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using MetaBrainz.Common;
using MetaBrainz.Common.Json;
using MetaBrainz.ListenBrainz.Json;
using MetaBrainz.ListenBrainz.Objects;

namespace MetaBrainz.ListenBrainz;

public sealed partial class ListenBrainz {

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

}
