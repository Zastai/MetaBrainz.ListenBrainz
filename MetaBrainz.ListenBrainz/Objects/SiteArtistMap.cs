using System;
using System.Collections.Generic;

using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

internal sealed class SiteArtistMap(DateTimeOffset lastUpdated, StatisticsRange range)
  : Statistics(lastUpdated, range), ISiteArtistMap {

  public IReadOnlyList<IArtistCountryInfo>? Countries { get; init; }

}
