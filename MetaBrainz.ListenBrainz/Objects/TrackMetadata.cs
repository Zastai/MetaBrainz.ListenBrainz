using System.Collections.Generic;
using System.Text.Json.Serialization;
using JetBrains.Annotations;
using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects {

  internal sealed class TrackMetadata : JsonBasedObject, ITrackMetaData {

    [JsonPropertyName("artist_name")]
    [UsedImplicitly]
    public string Artist { get; set; }

    public IReadOnlyDictionary<string, object> Info => this._info ??= JsonUtils.Unwrap(this.TheInfo);

    private Dictionary<string, object> _info;

    [JsonPropertyName("additional_info")]
    [UsedImplicitly]
    public Dictionary<string, object> TheInfo { get; set; }

    [JsonPropertyName("track_name")]
    [UsedImplicitly]
    public string Name { get; set; }

    [JsonPropertyName("release_name")]
    [UsedImplicitly]
    public string Release { get; set; }

  }

}
