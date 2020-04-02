using System;
using System.Text.Json.Serialization;

using JetBrains.Annotations;

using MetaBrainz.Common.Json;
using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects {

  [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
  internal sealed class Listen : JsonBasedObject, IListen {

    [JsonPropertyName("recording_msid")]
    public Guid? MessyRecordingId { get; set; }

    [JsonConverter(typeof(UnixTime.JsonConverter))]
    [JsonPropertyName("listened_at")]
    public DateTimeOffset? Timestamp { get; set; }

    [JsonPropertyName("track_metadata")]
    public ITrackInfo? Track  { get; set; }

    [JsonPropertyName("user_name")]
    public string? User { get; set; }

  }

}
