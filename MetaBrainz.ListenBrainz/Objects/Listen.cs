using System;
using MetaBrainz.ListenBrainz.Interfaces;
using Newtonsoft.Json;

namespace MetaBrainz.ListenBrainz.Objects {

  [JsonObject]
  internal sealed class Listen : IListen {

    [JsonProperty("recording_msid", Required = Required.Default)]
    public Guid? MessyRecording { get; private set; }

    [JsonIgnore]
    public DateTime Timestamp { get; private set; }

    public ITrackMetaData Track => this._track;

    [JsonProperty("track_metadata", Required = Required.Always)]
    private TrackMetadata _track = null;

    [JsonProperty("listened_at", Required = Required.Always)]
    public long UnixTimestamp {
      get => this._unixTimestamp;
      set => this.Timestamp = UnixTime.Convert(this._unixTimestamp = value);
    }

    private long _unixTimestamp = 0;

  }

}
