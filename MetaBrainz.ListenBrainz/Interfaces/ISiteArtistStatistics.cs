using System.Collections.Generic;

using JetBrains.Annotations;

namespace MetaBrainz.ListenBrainz.Interfaces;

/// <summary>The most-listened artists across several time ranges.</summary>
[PublicAPI]
public interface ISiteArtistStatistics : IStatistics {

  /// <summary>The (maximum) number of artists reported for each time range.</summary>
  int Count { get; }

  /// <summary>The offset of the artist lists (in each time range) from the start of the full set.</summary>
  int Offset { get; }

  /// <summary>Time ranges containing information about the most-listened artists.</summary>
  IReadOnlyList<IArtistTimeRange>? TimeRanges { get; }

}
