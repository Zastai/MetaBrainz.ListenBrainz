using System.Collections.Generic;

using JetBrains.Annotations;

namespace MetaBrainz.ListenBrainz.Interfaces;

/// <summary>The most-listened release groups in a particular timeframe.</summary>
[PublicAPI]
public interface IReleaseGroupStatistics : IStatistics {

  /// <summary>Information about the release groups.</summary>
  IReadOnlyList<IReleaseGroupInfo>? ReleaseGroups { get; }

  /// <summary>The offset of these statistics from the start of the full set.</summary>
  int? Offset { get; }

  /// <summary>The total number of (distinct) releases listened to, if available.</summary>
  int? TotalCount { get; }

}
