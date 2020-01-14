using System;
using System.Text.Json.Serialization;
using JetBrains.Annotations;
using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects {

  internal sealed class LatestImport : JsonBasedObject, ILatestImport {

    [JsonIgnore]
    public DateTime Timestamp { get; private set; }

    [JsonPropertyName("latest_import")]
    [UsedImplicitly]
    public long UnixTimestamp {
      get => this._unixTimestamp;
      set => this.Timestamp = UnixTime.Convert(this._unixTimestamp = value);
    }

    private long _unixTimestamp = 0;

    [JsonPropertyName("musicbrainz_id")]
    [UsedImplicitly]
    public string User { get; set; }

  }

}
