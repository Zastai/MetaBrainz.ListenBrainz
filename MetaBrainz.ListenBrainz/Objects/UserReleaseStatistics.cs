using System;
using System.Collections.Generic;

using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

internal sealed class UserReleaseStatistics : UserStatistics, IUserReleaseStatistics {

  public UserReleaseStatistics(DateTimeOffset lastUpdated, StatisticsRange range, string user)
  : base(lastUpdated, range, user)
  { }

  public IReadOnlyList<IReleaseInfo>? Releases { get; init; }

  public int? Offset { get; init; }

  public int? TotalCount { get; init; }

}
