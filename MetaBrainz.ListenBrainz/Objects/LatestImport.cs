using System;

using MetaBrainz.ListenBrainz.Interfaces;

using Newtonsoft.Json;

namespace MetaBrainz.ListenBrainz.Objects {

  [JsonObject]
  internal sealed class LatestImport : ILatestImport {

    [JsonIgnore]
    public DateTime Timestamp { get; private set; }

    [JsonProperty("latest_import", Required = Required.Always)]
    public long UnixTimestamp {
      get => this._unixTimestamp;
      set => this.Timestamp = UnixTime.Convert(this._unixTimestamp = value);
    }

    private long _unixTimestamp = 0;

    [JsonProperty("musicbrainz_id", Required = Required.Always)]
    public string User { get; private set; }

  }

}
