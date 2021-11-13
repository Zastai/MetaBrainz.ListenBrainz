using System;
using System.Collections.Generic;

using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

internal sealed class SiteArtistStatistics : Statistics, ISiteArtistStatistics {

  public SiteArtistStatistics(int count, DateTimeOffset lastUpdated, int offset, StatisticsRange range)
  : base(lastUpdated, range) {
    this.Count = count;
    this.Offset = offset;
  }

  public int Count { get; }

  public int Offset { get; }

  public IReadOnlyList<IArtistTimeRange>? TimeRanges { get; set; }

}
