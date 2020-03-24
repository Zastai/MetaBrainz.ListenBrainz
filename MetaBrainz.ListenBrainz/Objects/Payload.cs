using System.Text.Json.Serialization;

using JetBrains.Annotations;

using MetaBrainz.Common.Json;

namespace MetaBrainz.ListenBrainz.Objects {

  [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
  internal sealed class Payload<T> : JsonBasedObject where T : class {

    [JsonPropertyName("payload")]
    public T? Contents { get; set; }

  }

}
