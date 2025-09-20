using System;
using System.Collections.Generic;

using MetaBrainz.Common.Json;
using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

internal sealed class ReleaseGroupInfo : JsonBasedObject, IReleaseGroupInfo {

  public IReadOnlyList<Guid>? ArtistIds { get; init; }

  public string? ArtistName { get; init; }

  public long? CoverArtId { get; init; }

  public Guid? CoverArtReleaseId { get; init; }

  public IReadOnlyList<IArtistCredit>? Credits { get; init; }

  public Guid? Id { get; init; }

  public required int ListenCount { get; init; }

  public required string Name { get; init; }

}
