using System;

using JetBrains.Annotations;

namespace MetaBrainz.ListenBrainz.Interfaces {

  /// <summary>Information about listens recorded during a particular time range.</summary>
  [PublicAPI]
  public interface IListenTimeRange {

    /// <summary>A description of this time range.</summary>
    string Description { get; }

    /// <summary>The number of listens recorded in this time range.</summary>
    int ListenCount { get; }

    /// <summary>The ending timestamp for this time range.</summary>
    DateTimeOffset? RangeEnd { get; }

    /// <summary>The starting timestamp for this time range.</summary>
    DateTimeOffset? RangeStart { get; }

  }

}
