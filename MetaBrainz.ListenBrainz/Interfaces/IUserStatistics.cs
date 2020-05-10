using JetBrains.Annotations;

namespace MetaBrainz.ListenBrainz.Interfaces {

  /// <summary>A set of computed statistics for a user.</summary>
  [PublicAPI]
  public interface IUserStatistics : IStatistics {

    /// <summary>The user for whom the statistics were computed.</summary>
    string User { get; }

  }

}
