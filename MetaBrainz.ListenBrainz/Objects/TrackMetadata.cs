using System.Collections.Generic;
using System.Text.Json.Serialization;

using JetBrains.Annotations;

using MetaBrainz.Common.Json;
using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects {

  [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
  internal sealed class TrackMetadata : JsonBasedObject, ITrackMetaData {

    [JsonPropertyName("artist_name")]
    public string Artist { get; set; }

    [JsonPropertyName("additional_info")]
    public IReadOnlyDictionary<string, object> Info { get; set; }

    [JsonPropertyName("track_name")]
    public string Name { get; set; }

    [JsonPropertyName("release_name")]
    public string Release { get; set; }

  }

}
