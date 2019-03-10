using System.Collections.Generic;
using MetaBrainz.ListenBrainz.Interfaces;
using Newtonsoft.Json;

namespace MetaBrainz.ListenBrainz.Objects {

  [JsonObject]
  internal sealed class TrackMetadata : ITrackMetaData {

    [JsonProperty("artist_name", Required = Required.Always)]
    public string Artist { get; private set; }

    [JsonIgnore]
    public IReadOnlyDictionary<string, object> Info => this._info;

    [JsonProperty("additional_info", Required = Required.Default)]
    private Dictionary<string, object> _info = null;

    [JsonProperty("track_name", Required = Required.Always)]
    public string Name { get; private set; }

    [JsonProperty("release_name", Required = Required.Default)]
    public string Release { get; private set; }

  }

}
