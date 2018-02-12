using System;

using MetaBrainz.ListenBrainz.Interfaces;

using Newtonsoft.Json;

namespace MetaBrainz.ListenBrainz.Objects {

  [JsonObject]
  internal sealed class LatestImport : ILatestImport {
    
    [JsonProperty("latest_import", Required = Required.Always)]
    private long NumericTimestamp {
      set => this.Timestamp = UnixTime.Convert(value);
    }

    [JsonIgnore]
    public DateTime Timestamp { get; private set; }

    [JsonProperty("musicbrainz_id", Required = Required.Always)]
    public string User { get; private set; }

  }

}
