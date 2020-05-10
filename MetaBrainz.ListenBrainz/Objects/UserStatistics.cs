using System;

using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects {

  internal abstract class UserStatistics : Statistics, IUserStatistics {

    protected UserStatistics(DateTimeOffset lastUpdated, int offset, StatisticsRange range, string user) : base(lastUpdated, offset, range) {
      this.User = user;
    }

    public string User { get; }

  }

}
