using System;

using MetaBrainz.Common.Json;
using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

internal class TopArtist : JsonBasedObject, ITopArtist {

  public Guid? Id { get; init; }

  public required int ListenCount { get; init; }

  public required string Name { get; init; }

}
