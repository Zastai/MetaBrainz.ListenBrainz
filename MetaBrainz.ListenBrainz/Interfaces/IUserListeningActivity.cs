using System.Collections.Generic;

using JetBrains.Annotations;

namespace MetaBrainz.ListenBrainz.Interfaces;

/// <summary>Information about how many listens a user has submitted over a period of time.</summary>
[PublicAPI]
public interface IUserListeningActivity : IUserStatistics {

  /// <summary>The user's listening activity.</summary>
  IReadOnlyList<IListenTimeRange>? Activity { get; }

}
