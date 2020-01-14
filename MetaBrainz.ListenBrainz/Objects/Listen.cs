using System;
using System.Text.Json.Serialization;
using JetBrains.Annotations;
using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects {

  internal sealed class Listen : JsonBasedObject, IListen {

    [JsonPropertyName("recording_msid")]
    [UsedImplicitly]
    public Guid? MessyRecordingId { get; set; }

    [JsonIgnore]
    public DateTime Timestamp { get; private set; }

    public ITrackMetaData Track => this.TheTrack;

    [JsonPropertyName("track_metadata")]
    [UsedImplicitly]
    public TrackMetadata TheTrack { get; set; }

    [JsonPropertyName("listened_at")]
    [UsedImplicitly]
    public long UnixTimestamp {
      get => this._unixTimestamp;
      set => this.Timestamp = UnixTime.Convert(this._unixTimestamp = value);
    }

    private long _unixTimestamp = 0;

    [JsonPropertyName("user_name")]
    [UsedImplicitly]
    public string User { get; set; }

  }

}
