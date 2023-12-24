using System;
using System.Collections.Generic;

using JetBrains.Annotations;

using MetaBrainz.Common.Json;
using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
internal sealed class MusicBrainzIdMappings : JsonBasedObject, IMusicBrainzIdMappings {

  public IReadOnlyList<Guid>? ArtistIds { get; init; }

  public long? CoverArtId { get; init; }

  public Guid? CoverArtReleaseId { get; init; }

  public IReadOnlyList<IArtistCredit>? Credits { get; init; }

  public Guid? RecordingId { get; init; }

  public string? RecordingName { get; init; }

  public Guid? ReleaseId { get; init; }

}
