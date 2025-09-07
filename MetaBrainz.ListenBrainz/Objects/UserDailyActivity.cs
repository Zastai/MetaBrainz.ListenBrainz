using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

internal sealed class UserDailyActivity : UserStatistics, IUserDailyActivity {

  public IDailyActivity? Activity { get; init; }

}
