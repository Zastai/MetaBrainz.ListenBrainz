using JetBrains.Annotations;

namespace MetaBrainz.ListenBrainz.Interfaces;

/// <summary>Information about how a user's listens are spread across the days of the week.</summary>
[PublicAPI]
public interface IUserDailyActivity : IUserStatistics {

  /// <summary>The user's daily activity.</summary>
  IDailyActivity? Activity { get; }

}
