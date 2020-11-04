using System;
using System.Collections.Generic;

using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects {

  internal sealed class UserReleaseStatistics : UserStatistics, IUserReleaseStatistics {

    public UserReleaseStatistics(DateTimeOffset lastUpdated, StatisticsRange range, string user)
    : base(lastUpdated, range, user)
    { }

    public IReadOnlyList<IReleaseInfo>? Releases { get; set; }

    public int? Offset { get; set; }

    public int? TotalCount { get; set; }

  }

}
