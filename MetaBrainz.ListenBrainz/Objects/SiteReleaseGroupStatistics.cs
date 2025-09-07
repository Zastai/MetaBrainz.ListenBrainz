using System;
using System.Collections.Generic;

using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

internal sealed class SiteReleaseGroupStatistics(DateTimeOffset lastUpdated, StatisticsRange range)
  : Statistics(lastUpdated, range), ISiteReleaseGroupStatistics {

  public IReadOnlyList<IReleaseGroupInfo>? ReleaseGroups { get; init; }

  public int? Offset { get; init; }

  public int? TotalCount { get; init; }

}
