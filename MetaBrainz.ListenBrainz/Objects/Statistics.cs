using System;

using MetaBrainz.Common.Json;
using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

internal abstract class Statistics : JsonBasedObject, IStatistics {

  protected Statistics(DateTimeOffset lastUpdated, StatisticsRange range) {
    this.LastUpdated = lastUpdated;
    this.Range = range;
  }

  public DateTimeOffset LastUpdated { get; }

  public DateTimeOffset? NewestListen { get; init; }

  public DateTimeOffset? OldestListen { get; init; }

  public StatisticsRange Range { get; }

}
