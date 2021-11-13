using JetBrains.Annotations;

using MetaBrainz.Common.Json;
using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
internal sealed class TrackInfo : JsonBasedObject, ITrackInfo {

  public TrackInfo(string name, string artist, IAdditionalInfo info) {
    this.Artist = artist;
    this.Name = name;
    this.AdditionalInfo = info;
  }

  public IAdditionalInfo AdditionalInfo { get; }

  public string Artist { get; }

  public string Name { get; }

  public string? Release { get; set; }

}
