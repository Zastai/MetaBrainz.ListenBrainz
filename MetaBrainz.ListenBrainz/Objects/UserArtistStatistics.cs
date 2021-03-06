using System;
using System.Collections.Generic;

using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects {

  internal sealed class UserArtistStatistics : UserStatistics, IUserArtistStatistics {

    public UserArtistStatistics(DateTimeOffset lastUpdated, StatisticsRange range, string user)
    : base(lastUpdated, range, user)
    { }

    public IReadOnlyList<IArtistInfo>? Artists { get; set; }

    public int? Offset { get; set; }

    public int? TotalCount { get; set; }

  }

}
