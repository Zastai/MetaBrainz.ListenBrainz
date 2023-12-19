using System;

using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

internal sealed class UserDailyActivity : UserStatistics, IUserDailyActivity {

  public UserDailyActivity(DateTimeOffset lastUpdated, StatisticsRange range, string user)
  : base(lastUpdated, range, user)
  { }

  public IDailyActivity? Activity { get; init; }

}
