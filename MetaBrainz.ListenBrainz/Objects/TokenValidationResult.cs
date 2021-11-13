using System.Net;

using MetaBrainz.Common.Json;
using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

internal sealed class TokenValidationResult : JsonBasedObject, ITokenValidationResult {

  public TokenValidationResult(HttpStatusCode code, string message) {
    this.Code = code;
    this.Message = message;
  }

  public HttpStatusCode Code { get; }

  public string Message { get; }

  public string? User { get; set; }

  public bool? Valid { get; set; }

}
