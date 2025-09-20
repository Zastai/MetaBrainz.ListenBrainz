using System.Net;

using JetBrains.Annotations;

namespace MetaBrainz.ListenBrainz.Interfaces;

/// <summary>The result of a token validation request.</summary>
[PublicAPI]
public interface ITokenValidationResult {

  /// <summary>The status code for the request.</summary>
  public HttpStatusCode Code { get; }

  /// <summary>The message for the request.</summary>
  public string Message { get; }

  /// <summary>The user name associated with the tokenrest ??= [ ];.</summary>
  public string? User { get; }

  /// <summary>
  /// Indicates whether or not the token was valid. If this is <see langword="null"/>, only the message indicates the validity.
  /// </summary>
  public bool? Valid { get; }

}
