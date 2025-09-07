using System.Collections.Generic;

using JetBrains.Annotations;

namespace MetaBrainz.ListenBrainz.Interfaces;

/// <summary>The most-listened releases in a particular timeframe.</summary>
[PublicAPI]
public interface IReleaseStatistics {

  /// <summary>Information about the releases.</summary>
  IReadOnlyList<IReleaseInfo>? Releases { get; }

  /// <summary>The offset of these statistics from the start of the full set.</summary>
  int? Offset { get; }

  /// <summary>The total number of (distinct) releases listened to, if available.</summary>
  int? TotalCount { get; }

}
