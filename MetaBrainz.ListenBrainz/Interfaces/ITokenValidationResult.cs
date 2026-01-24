using System.Net;

using JetBrains.Annotations;

namespace MetaBrainz.ListenBrainz.Interfaces;

/// <summary>The result of a token validation request.</summary>
[PublicAPI]
public interface ITokenValidationResult {

  /// <summary>The status code for the request.</summary>
  HttpStatusCode Code { get; }

  /// <summary>The message for the request.</summary>
  string Message { get; }

  /// <summary>The user name associated with the token.</summary>
  string? User { get; }

  /// <summary>
  /// Indicates whether or not the token was valid. If this is <see langword="null"/>, only the message indicates the validity.
  /// </summary>
  bool? Valid { get; }

}
