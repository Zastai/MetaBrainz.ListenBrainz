using System;
using System.Linq;
using System.Net.Http.Headers;

using JetBrains.Annotations;

namespace MetaBrainz.ListenBrainz {

  /// <summary>
  /// Information about any rate limiting that is in effect, as returned in the response headers for web service requests.
  /// </summary>
  [PublicAPI]
  public readonly struct RateLimitInfo {

    /// <summary>The total number of requests allowed in the current time window, if available.</summary>
    /// <remarks>Corresponds to the value of the <code>X-RateLimit-Limit</code> header.</remarks>
    public int? AllowedRequests { get; }

    /// <summary>The date and time at which the last request was made.</summary>
    /// <remarks>
    /// This is the (client-side) timestamp to which <see cref="ResetIn"/> can be added to determine the end of the window.
    /// </remarks>
    public DateTimeOffset LastRequest { get; }

    /// <summary>The number of requests remaining in the current time window, if available.</summary>
    /// <remarks>Corresponds to the value of the <code>X-RateLimit-Remaining</code> header.</remarks>
    public int? RemainingRequests { get; }

    /// <summary>The date and time at which the current time window expires, if available.</summary>
    /// <remarks>
    /// Corresponds to the value of the <code>X-RateLimit-Reset</code> header.<br/>
    /// Provided for compatibility only; use of <see cref="ResetIn"/> is recommended to avoid issues with clients with clocks that
    /// are not correctly set.
    /// </remarks>
    public DateTimeOffset? ResetAt { get; }

    /// <summary>The number of seconds remaining until the current time window expires, if available.</summary>
    /// <remarks>Corresponds to the value of the <code>X-RateLimit-Reset-In</code> header.</remarks>
    public int? ResetIn { get; }

    /// <summary>Extracts rate limiting information from the headers of a web service response.</summary>
    /// <param name="headers">The web service response headers to get the rate limit information from.</param>
    /// <returns>
    /// Rate limit information, as extracted from <paramref name="headers"/>.<br/>
    /// <see cref="LastRequest"/> will be set to the current (UTC) date/time.
    /// </returns>
    public RateLimitInfo(HttpResponseHeaders headers) {
      this.AllowedRequests = RateLimitInfo.GetIntHeader(headers, "X-RateLimit-Limit");
      this.LastRequest = DateTimeOffset.UtcNow;
      this.RemainingRequests = RateLimitInfo.GetIntHeader(headers, "X-RateLimit-Remaining");
      this.ResetAt = RateLimitInfo.GetUnixTimeHeader(headers, "X-RateLimit-Reset");
      this.ResetIn = RateLimitInfo.GetIntHeader(headers, "X-RateLimit-Reset-In");
    }

    private static int? GetIntHeader(HttpResponseHeaders headers, string header) {
      if (!headers.Contains(header))
        return null;
      var text = headers.GetValues(header).FirstOrDefault();
      if (text != null && int.TryParse(text, out var value))
        return value;
      return null;
    }

    private static DateTimeOffset? GetUnixTimeHeader(HttpResponseHeaders headers, string header) {
      if (!headers.Contains(header))
        return null;
      var text = headers.GetValues(header).FirstOrDefault();
      if (text != null && long.TryParse(text, out var value))
        return UnixTime.Convert(value);
      return null;
    }

  }

}
