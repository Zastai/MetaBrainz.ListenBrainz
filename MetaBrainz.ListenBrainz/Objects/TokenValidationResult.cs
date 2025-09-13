using System.Net;

using MetaBrainz.Common.Json;
using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

internal sealed class TokenValidationResult : JsonBasedObject, ITokenValidationResult {

  public required HttpStatusCode Code { get; init; }

  public required string Message { get; init; }

  public string? User { get; init; }

  public bool? Valid { get; init; }

}
