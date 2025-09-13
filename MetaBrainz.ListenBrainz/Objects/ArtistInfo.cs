using System;
using System.Collections.Generic;

using MetaBrainz.Common.Json;
using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

internal sealed class ArtistInfo : JsonBasedObject, IArtistInfo {

  public Guid? Id { get; init; }

  public IReadOnlyList<Guid>? Ids { get; init; }

  public required int ListenCount { get; init; }

  public Guid? MessyId { get; init; }

  public required string Name { get; init; }

}
