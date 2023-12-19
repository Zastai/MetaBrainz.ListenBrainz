using JetBrains.Annotations;

using MetaBrainz.Common.Json;

namespace MetaBrainz.ListenBrainz.Objects;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
internal sealed class ErrorInfo : JsonBasedObject {

  public ErrorInfo(int code, string error) {
    this.Code = code;
    this.Error = error;
  }

  public int Code { get; }

  public string Error { get; }

}
