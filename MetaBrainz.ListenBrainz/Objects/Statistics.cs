using System;

using MetaBrainz.Common.Json;
using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

internal abstract class Statistics : JsonBasedObject, IStatistics {

  public required DateTimeOffset LastUpdated { get; init; }

  public DateTimeOffset? NewestListen { get; init; }

  public DateTimeOffset? OldestListen { get; init; }

  public required StatisticsRange Range { get; init; }

}
