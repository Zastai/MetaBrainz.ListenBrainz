using System.Collections.Generic;

using JetBrains.Annotations;

namespace MetaBrainz.ListenBrainz.Interfaces;

/// <summary>A user's most-listened releases.</summary>
[PublicAPI]
public interface IUserReleaseGroupStatistics : IUserStatistics {

  /// <summary>Information about the release groups.</summary>
  IReadOnlyList<IReleaseGroupInfo>? ReleaseGroups { get; }

  /// <summary>The offset of these statistics from the start of the full set.</summary>
  int? Offset { get; }

  /// <summary>The total number of (distinct) releases listened to, if available.</summary>
  int? TotalCount { get; }

}
