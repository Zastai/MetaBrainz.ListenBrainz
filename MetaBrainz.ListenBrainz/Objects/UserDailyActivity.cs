using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

internal sealed class UserDailyActivity : Statistics, IUserDailyActivity {

  public IDailyActivity? Activity { get; init; }

}
