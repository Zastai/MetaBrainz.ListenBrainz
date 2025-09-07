using System;
using System.Collections.Generic;

using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

internal sealed class UserArtistStatistics(int count, int totalCount, DateTimeOffset lastUpdated, int offset, StatisticsRange range,
                                           string user)
  : UserStatistics(lastUpdated, range, user), IUserArtistStatistics {

  public IReadOnlyList<IArtistInfo>? Artists { get; init; }

  public int Count { get; } = count;

  public int Offset { get; } = offset;

  public int TotalCount { get; } = totalCount;

}
