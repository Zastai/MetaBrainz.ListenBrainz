using System;
using System.Net;

using JetBrains.Annotations;

namespace MetaBrainz.ListenBrainz {

  /// <summary>
  /// Information about any rate limiting that is in effect, as returned in the response headers for web service requests.
  /// </summary>
  [PublicAPI]
  public struct RateLimitInfo {

    /// <summary>The total number of requests allowed in the current time window.</summary>
    /// <remarks>Corresponds to the value of the <code>X-RateLimit-Limit</code> header.</remarks>
    public int AllowedRequests { get; private set; }

    /// <summary>The date and time at which the last request was made.</summary>
    /// <remarks>
    /// This is the (client-side) timestamp to which <see cref="ResetIn"/> can be added to determine the end of the window.
    /// </remarks>
    public DateTimeOffset LastRequest { get; private set; }

    /// <summary>The number of requests remaining in the current time window.</summary>
    /// <remarks>Corresponds to the value of the <code>X-RateLimit-Remaining</code> header.</remarks>
    public int RemainingRequests { get; private set; }

    /// <summary>The date and time at which the current time window expires.</summary>
    /// <remarks>
    /// Corresponds to the value of the <code>X-RateLimit-Reset</code> header.<br/>
    /// Provided for compatibility only; use of <see cref="ResetIn"/> is recommended to avoid issues with clients with clocks that
    /// are not correctly set.
    /// </remarks>
    public DateTimeOffset ResetAt { get; private set; }

    /// <summary>The number of seconds remaining until the current time window expires.</summary>
    /// <remarks>Corresponds to the value of the <code>X-RateLimit-Reset-In</code> header.</remarks>
    public int ResetIn { get; private set; }

    /// <summary>Extracts rate limiting information from a set of web request headers.</summary>
    /// <param name="headers">The web request headers to get the rate limit information from.</param>
    /// <returns>
    /// Rate limit information, as extracted from <paramref name="headers"/>.<br/>
    /// <see cref="LastRequest"/> will be set to the current (UTC) date/time.
    /// <see cref="AllowedRequests"/>, <see cref="RemainingRequests"/>, and <see cref="ResetIn"/> will be set to -1 if not
    /// available; <see cref="ResetAt"/> will be set to <see cref="LastRequest"/> when not available.
    /// </returns>
    public static RateLimitInfo From(WebHeaderCollection? headers) {
      var now = DateTimeOffset.UtcNow;
      if (headers == null) {
        return new RateLimitInfo {
          // @formatter:off
          AllowedRequests   = -1,
          LastRequest       = now,
          RemainingRequests = -1,
          ResetAt           = now,
          ResetIn           = -1,
          // @formatter:on
        };
      }
      var rli = new RateLimitInfo { LastRequest = now };
      {
        var text = headers.Get("X-RateLimit-Limit");
        if (text == null || !int.TryParse(text, out var value))
          rli.AllowedRequests = -1;
        else
          rli.AllowedRequests = value;
      }
      {
        var text = headers.Get("X-RateLimit-Remaining");
        if (text == null || !int.TryParse(text, out var value))
          rli.RemainingRequests = -1;
        else
          rli.RemainingRequests = value;
      }
      {
        var text = headers.Get("X-RateLimit-Reset");
        if (text == null || !long.TryParse(text, out var value))
          rli.ResetAt = rli.LastRequest;
        else
          rli.ResetAt = UnixTime.Convert(value);
      }
      {
        var text = headers.Get("X-RateLimit-Reset-In");
        if (text == null || !int.TryParse(text, out var value))
          rli.ResetIn = -1;
        else
          rli.ResetIn = value;
      }
      return rli;
    }

  }

}
