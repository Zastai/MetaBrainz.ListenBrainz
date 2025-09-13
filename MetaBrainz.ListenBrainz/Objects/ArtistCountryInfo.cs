using System.Collections.Generic;

using MetaBrainz.Common.Json;
using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

internal sealed class ArtistCountryInfo : JsonBasedObject, IArtistCountryInfo {

  public required int ArtistCount { get; init; }

  public IReadOnlyList<IArtistInfo>? Artists { get; init; }

  public required string Country { get; init; }

  public required int ListenCount { get; init; }

}
