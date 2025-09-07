using System;
using System.Collections.Generic;

using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

internal sealed class SiteRecordingStatistics(DateTimeOffset lastUpdated, StatisticsRange range)
  : Statistics(lastUpdated, range), ISiteRecordingStatistics {

  public IReadOnlyList<IRecordingInfo>? Recordings { get; init; }

  public int? Offset { get; init; }

  public int? TotalCount { get; init; }

}
