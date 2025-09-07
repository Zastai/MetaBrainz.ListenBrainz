using System.Collections.Generic;

using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

internal sealed class UserListeningActivity : UserStatistics, IUserListeningActivity {

  public IReadOnlyList<IListenTimeRange>? Activity { get; init; }

}
