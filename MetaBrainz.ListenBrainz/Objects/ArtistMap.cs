using System.Collections.Generic;

using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

internal sealed class ArtistMap : Statistics, IArtistMap {

  public IReadOnlyList<IArtistCountryInfo>? Countries { get; init; }

}
