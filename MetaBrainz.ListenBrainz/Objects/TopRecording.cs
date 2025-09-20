using System;
using System.Collections.Generic;

using MetaBrainz.Common.Json;
using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

internal sealed class TopRecording : JsonBasedObject, ITopRecording {

  public IReadOnlyList<Guid>? ArtistIds { get; init; }

  public string? ArtistName { get; init; }

  public long? CoverArtId { get; init; }

  public Guid? CoverArtReleaseGroupId { get; init; }

  public IReadOnlyList<IArtistCredit>? Credits { get; init; }

  public Guid? Id { get; init; }

  public required int ListenCount { get; init; }

  public required string Name { get; init; }

  public Guid? ReleaseId { get; init; }

  public string? ReleaseName { get; init; }

}
