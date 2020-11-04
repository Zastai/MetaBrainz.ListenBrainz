using System;

using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects {

  internal abstract class UserStatistics : Statistics, IUserStatistics {

    protected UserStatistics(DateTimeOffset lastUpdated, StatisticsRange range, string user) : base(lastUpdated, range) {
      this.User = user;
    }

    public string User { get; }

  }

}
