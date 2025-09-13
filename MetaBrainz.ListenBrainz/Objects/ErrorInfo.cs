using JetBrains.Annotations;

using MetaBrainz.Common.Json;

namespace MetaBrainz.ListenBrainz.Objects;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
internal sealed class ErrorInfo : JsonBasedObject {

  public required int Code { get; init; }

  public required string Error { get; init; }

}
