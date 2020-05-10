using System;

using JetBrains.Annotations;

using MetaBrainz.Common.Json;

namespace MetaBrainz.ListenBrainz.Interfaces {

  /// <summary>A set of computed statistics.</summary>
  [PublicAPI]
  public interface IStatistics : IJsonBasedObject {

    /// <summary>The timestamp at which the statistics were last updated.</summary>
    DateTimeOffset LastUpdated { get; }

    /// <summary>The offset of these statistics from the start of the full set.</summary>
    int Offset { get; }

    /// <summary>The range of data used when computing the statistics.</summary>
    /// <remarks>
    /// If this is <see cref="StatisticsRange.Unknown"/>, the actual text value will be stored in
    /// <see cref="IJsonBasedObject.UnhandledProperties"/> under the "range" key.
    /// </remarks>
    StatisticsRange Range { get; }

  }

}