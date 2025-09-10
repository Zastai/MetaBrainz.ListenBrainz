using System.Collections.Generic;

using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

internal sealed class SiteArtistMap : Statistics, ISiteArtistMap {

  public IReadOnlyList<IArtistCountryInfo>? Countries { get; init; }

}
