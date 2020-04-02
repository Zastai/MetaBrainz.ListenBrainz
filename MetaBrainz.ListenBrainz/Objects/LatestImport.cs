using System;
using System.Text.Json.Serialization;

using JetBrains.Annotations;

using MetaBrainz.Common.Json;
using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects {

  [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
  internal sealed class LatestImport : JsonBasedObject, ILatestImport {

    [JsonConverter(typeof(UnixTime.JsonConverter))]
    [JsonPropertyName("latest_import")]
    public DateTimeOffset? Timestamp { get; set; }

    public long? UnixTimestamp => UnixTime.Convert(this.Timestamp);

    [JsonPropertyName("musicbrainz_id")]
    public string? User { get; set; }

  }

}
