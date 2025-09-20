using System.Collections.Generic;

using JetBrains.Annotations;

namespace MetaBrainz.ListenBrainz.Interfaces;

/// <summary>Information about the most-listened recordings ("tracks").</summary>
[PublicAPI]
public interface IRecordingStatistics : IStatistics {

  /// <summary>Information about the recordings.</summary>
  IReadOnlyList<IRecordingInfo>? Recordings { get; }

  /// <summary>The offset of these statistics from the start of the full set.</summary>
  int? Offset { get; }

  /// <summary>The total number of (distinct) recordings listened to.</summary>
  int? TotalCount { get; }

}
