using System;
using System.Collections.Generic;

using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

internal sealed class SiteReleaseStatistics(DateTimeOffset lastUpdated, StatisticsRange range)
  : Statistics(lastUpdated, range), ISiteReleaseStatistics {

  public IReadOnlyList<IReleaseInfo>? Releases { get; init; }

  public int? Offset { get; init; }

  public int? TotalCount { get; init; }

}
