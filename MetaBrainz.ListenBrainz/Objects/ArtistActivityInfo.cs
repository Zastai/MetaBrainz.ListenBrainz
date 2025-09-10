using System;
using System.Collections.Generic;

using MetaBrainz.Common.Json;
using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

internal sealed class ArtistActivityInfo : JsonBasedObject, IArtistActivityInfo {

  public required IReadOnlyList<IAlbumInfo> Albums { get; init; }

  public Guid? Id { get; init; }

  public required int ListenCount { get; init; }

  public required string Name { get; init; }

}
