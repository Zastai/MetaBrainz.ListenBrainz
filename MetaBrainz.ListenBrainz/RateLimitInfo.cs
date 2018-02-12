using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace MetaBrainz.ListenBrainz {

  /// <summary>
  /// Information about any rate limiting that is in effect, as returned in the response headers for web service requests.
  /// </summary>
  [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
  [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
  public struct RateLimitInfo {

    /// <summary>The total number of requests allowed in the current time window.</summary>
    /// <remarks>Corresponds to the value of the <code>X-RateLimit-Limit</code> header.</remarks>
    public int AllowedRequests { get; private set; }

    /// <summary>The date and time (UTC) at which the last request was made.</summary>
    /// <remarks>
    /// This is the (client-side) timestamp to which <see cref="ResetIn"/> can be added to determine the end of the window.
    /// </remarks>
    public DateTime LastRequest { get; private set; }

    /// <summary>The number of requests remaining in the current time window.</summary>
    /// <remarks>Corresponds to the value of the <code>X-RateLimit-Remaining</code> header.</remarks>
    public int RemainingRequests { get; private set; }
    
    /// <summary>The date and time (UTC) at which the current time window expires.</summary>
    /// <remarks>
    /// Corresponds to the value of the <code>X-RateLimit-Reset</code> header.<br/>
    /// Provided for compatibility only; use of <see cref="ResetIn"/> is recommended to avoid issues with clients with clocks that
    /// are not correctly set.
    /// </remarks>
    public DateTime ResetAt { get; private set; }

    /// <summary>The number of seconds remaining until the current time window expires.</summary>
    /// <remarks>Corresponds to the value of the <code>X-RateLimit-Reset-In</code> header.</remarks>
    public int ResetIn { get; private set; }
    
    internal void SetFrom(WebHeaderCollection headers) {
      this.LastRequest = DateTime.UtcNow;
      if (headers == null) {
        this.AllowedRequests   = 0;
        this.RemainingRequests = 0;
        this.ResetAt           = DateTime.MaxValue;
        this.ResetIn           = int.MaxValue;
        return;
      }
      {
        var text = headers.Get("X-RateLimit-Limit");
        if (text == null || !int.TryParse(text, out var value))
          this.AllowedRequests = 0;
        else
          this.AllowedRequests = value;
      }
      {
        var text = headers.Get("X-RateLimit-Remaining");
        if (text == null || !int.TryParse(text, out var value))
          this.RemainingRequests = 0;
        else
          this.RemainingRequests = value;
      }
      {
        var text = headers.Get("X-RateLimit-Reset");
        if (text == null || !long.TryParse(text, out var value))
          this.ResetAt = DateTime.MaxValue;
        else
          this.ResetAt = UnixTime.Convert(value);
      }
      {
        var text = headers.Get("X-RateLimit-Reset-In");
        if (text == null || !int.TryParse(text, out var value))
          this.ResetIn = 0;
        else
          this.ResetIn = value;
      }
    }
    
  }

}
