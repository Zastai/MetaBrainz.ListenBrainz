using System;
using System.Collections.Generic;

using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

internal sealed class UserArtistStatistics : UserStatistics, IUserArtistStatistics {

  public UserArtistStatistics(int count, int totalCount, DateTimeOffset lastUpdated, int offset, StatisticsRange range, string user)
  : base(lastUpdated, range, user)
  {
    this.Count = count;
    this.Offset = offset;
    this.TotalCount = totalCount;
  }

  public IReadOnlyList<IArtistInfo>? Artists { get; set; }

  public int Count { get; }

  public int Offset { get; }

  public int TotalCount { get; }

}
