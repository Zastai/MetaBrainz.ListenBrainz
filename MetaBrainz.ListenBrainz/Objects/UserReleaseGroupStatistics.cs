using System;
using System.Collections.Generic;

using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

internal sealed class UserReleaseGroupStatistics : UserStatistics, IUserReleaseGroupStatistics {

  public UserReleaseGroupStatistics(DateTimeOffset lastUpdated, StatisticsRange range, string user)
  : base(lastUpdated, range, user)
  { }

  public IReadOnlyList<IReleaseGroupInfo>? ReleaseGroups { get; init; }

  public int? Offset { get; init; }

  public int? TotalCount { get; init; }

}
