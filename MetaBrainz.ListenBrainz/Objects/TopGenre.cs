using MetaBrainz.Common.Json;
using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

internal sealed class TopGenre : JsonBasedObject, ITopGenre {

  public required string Genre { get; init; }

  public required int ListenCount { get; init; }

  public required decimal Percentage { get; init; }

}
