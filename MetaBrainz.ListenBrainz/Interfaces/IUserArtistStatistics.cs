using System.Collections.Generic;

using JetBrains.Annotations;

namespace MetaBrainz.ListenBrainz.Interfaces;

/// <summary>A user's most-listened artists.</summary>
[PublicAPI]
public interface IUserArtistStatistics : IUserStatistics {

  /// <summary>Information about the artists.</summary>
  IReadOnlyList<IArtistInfo>? Artists { get; }

  /// <summary>The offset of these statistics from the start of the full set.</summary>
  int? Offset { get; }

  /// <summary>The total number of (distinct) artists listened to, if available.</summary>
  int? TotalCount { get; }

}
