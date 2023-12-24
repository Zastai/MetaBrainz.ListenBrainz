using System;
using System.Collections.Generic;

using MetaBrainz.Common.Json;
using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

internal sealed class ArtistInfo : JsonBasedObject, IArtistInfo {

  public ArtistInfo(string name, int listenCount) {
    this.Name = name;
    this.ListenCount = listenCount;
  }

  public Guid? Id { get; init; }

  public IReadOnlyList<Guid>? Ids { get; init; }

  public int ListenCount { get; }

  public Guid? MessyId { get; init; }

  public string Name { get; }

}
