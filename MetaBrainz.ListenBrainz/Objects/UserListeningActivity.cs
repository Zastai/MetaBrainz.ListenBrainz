using System;
using System.Collections.Generic;

using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

internal sealed class UserListeningActivity : UserStatistics, IUserListeningActivity {

  public UserListeningActivity(DateTimeOffset lastUpdated, StatisticsRange range, string user)
  : base(lastUpdated, range, user)
  { }

  public IReadOnlyList<IListenTimeRange>? Activity { get; set; }

}
