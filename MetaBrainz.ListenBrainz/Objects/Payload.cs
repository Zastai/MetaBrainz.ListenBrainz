using System.Text.Json.Serialization;

using JetBrains.Annotations;

namespace MetaBrainz.ListenBrainz.Objects {

  internal sealed class Payload<T> : JsonBasedObject {

    [JsonPropertyName("payload")]
    [UsedImplicitly]
    public T Contents { get; set; }

  }

}
