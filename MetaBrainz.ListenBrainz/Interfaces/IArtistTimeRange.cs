using System;
using System.Collections.Generic;

using JetBrains.Annotations;

namespace MetaBrainz.ListenBrainz.Interfaces;

/// <summary>Information about artists listened to during a particular time range.</summary>
[PublicAPI]
public interface IArtistTimeRange {

  /// <summary>A description of this time range.</summary>
  string Description { get; }

  /// <summary>The artists that were listened to in this time range.</summary>
  IReadOnlyList<IArtistInfo>? Artists { get; }

  /// <summary>The ending timestamp for this time range.</summary>
  DateTimeOffset? RangeEnd { get; }

  /// <summary>The starting timestamp for this time range.</summary>
  DateTimeOffset? RangeStart { get; }

}
