using System;

using MetaBrainz.Common.Json;
using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

internal sealed class AlbumInfo : JsonBasedObject, IAlbumInfo {

  public Guid? Id { get; init; }

  public required int ListenCount { get; init; }

  public required string Title { get; init; }

}
