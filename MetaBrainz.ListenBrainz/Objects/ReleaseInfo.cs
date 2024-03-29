using System;
using System.Collections.Generic;

using MetaBrainz.Common.Json;
using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

internal sealed class ReleaseInfo : JsonBasedObject, IReleaseInfo {

  public ReleaseInfo(string name, int listenCount) {
    this.Name = name;
    this.ListenCount = listenCount;
  }

  public IReadOnlyList<Guid>? ArtistIds { get; init; }

  public Guid? ArtistMessyId { get; init; }

  public string? ArtistName { get; init; }

  public long? CoverArtId { get; init; }

  public Guid? CoverArtReleaseId { get; init; }

  public IReadOnlyList<IArtistCredit>? Credits { get; init; }

  public Guid? Id { get; init; }

  public int ListenCount { get; }

  public Guid? MessyId { get; init; }

  public string Name { get; }

}
