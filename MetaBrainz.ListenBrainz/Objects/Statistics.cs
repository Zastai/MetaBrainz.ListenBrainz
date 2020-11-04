using System;

using MetaBrainz.Common.Json;
using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects {

  internal abstract class Statistics : JsonBasedObject, IStatistics {

    protected Statistics(DateTimeOffset lastUpdated, int offset, StatisticsRange range) {
      this.LastUpdated = lastUpdated;
      this.Offset = offset;
      this.Range = range;
    }

    public DateTimeOffset LastUpdated { get; }

    public DateTimeOffset? NewestListen { get; set; }

    public int Offset { get; }

    public DateTimeOffset? OldestListen { get; set; }

    public StatisticsRange Range { get; }

  }

}
