using System.Collections.Generic;

using JetBrains.Annotations;

namespace MetaBrainz.ListenBrainz.Interfaces;

/// <summary>Statistics about how many times particular artists were listened to.</summary>
[PublicAPI]
public interface IArtistStatistics : IStatistics {

  /// <summary>Information about the artists.</summary>
  IReadOnlyList<IArtistInfo>? Artists { get; }

  /// <summary>The (maximum) number of artists reported for each time range.</summary>
  int Count { get; }

  /// <summary>The offset of the artist lists (in each time range) from the start of the full set.</summary>
  int Offset { get; }

  /// <summary>The total number of (distinct) artists listened to.</summary>
  public int TotalCount { get; }

}
