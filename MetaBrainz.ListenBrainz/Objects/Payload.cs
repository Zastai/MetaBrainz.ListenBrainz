using Newtonsoft.Json;

namespace MetaBrainz.ListenBrainz.Objects {

  [JsonObject]
  internal sealed class Payload<T> {

    [JsonProperty("payload", Required = Required.Always)]
    public T Contents { get; private set; }

  }

}
