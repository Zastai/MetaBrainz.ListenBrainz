using System;
using System.Net;

namespace MetaBrainz.ListenBrainz;

/// <summary>An error reported by the ListenBrainz web service.</summary>
public sealed class QueryException : Exception {

  /// <summary>The HTTP status code for the exception.</summary>
  public readonly HttpStatusCode Code;

  /// <summary>The reason phrase for the exception, if available.</summary>
  public readonly string? Reason;

  /// <summary>Creates a new <see cref="QueryException"/> instance.</summary>
  /// <param name="code">The HTTP message code for the error.</param>
  /// <param name="reason">The reason phrase for the error.</param>
  /// <param name="message">A further error message.</param>
  /// <param name="cause">The exception that caused the error (if any).</param>
  public QueryException(HttpStatusCode code, string? reason = null, string? message = null, Exception? cause = null)
  : base(message ?? reason, cause) {
    this.Code = code;
    this.Reason = reason;
  }

}
