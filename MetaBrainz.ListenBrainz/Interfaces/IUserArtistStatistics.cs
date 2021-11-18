using JetBrains.Annotations;

namespace MetaBrainz.ListenBrainz.Interfaces;

/// <summary>A user's most-listened artists.</summary>
[PublicAPI]
public interface IUserArtistStatistics : IArtistStatistics, IUserStatistics {

  /// <summary>The total number of (distinct) artists listened to, if available.</summary>
  int TotalCount { get; }

}
