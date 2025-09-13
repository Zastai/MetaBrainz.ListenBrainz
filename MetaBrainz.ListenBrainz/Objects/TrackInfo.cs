using JetBrains.Annotations;

using MetaBrainz.Common.Json;
using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
internal sealed class TrackInfo : JsonBasedObject, ITrackInfo {

  public required IAdditionalInfo AdditionalInfo { get; init; }

  public required string Artist { get; init; }

  public IMusicBrainzIdMappings? MusicBrainzIdMappings { get; init; }

  public required string Name { get; init; }

  public string? Release { get; init; }

}
