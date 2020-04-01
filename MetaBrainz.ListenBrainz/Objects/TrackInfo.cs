using System.Text.Json.Serialization;

using JetBrains.Annotations;

using MetaBrainz.Common.Json;
using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects {

  [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
  internal sealed class TrackInfo : JsonBasedObject, ITrackInfo {

    [JsonPropertyName("additional_info")]
    public IAdditionalInfo? AdditionalInfo { get; set; }

    [JsonPropertyName("artist_name")]
    public string? Artist { get; set; }

    [JsonPropertyName("track_name")]
    public string? Name { get; set; }

    [JsonPropertyName("release_name")]
    public string? Release { get; set; }

  }

}
