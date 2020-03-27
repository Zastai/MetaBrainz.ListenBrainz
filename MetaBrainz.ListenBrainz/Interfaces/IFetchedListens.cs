using System;
using System.Collections.Generic;

using JetBrains.Annotations;

using MetaBrainz.Common.Json;

namespace MetaBrainz.ListenBrainz.Interfaces {

  /// <summary>Information about a set of fetched listens.</summary>
  [PublicAPI]
  public interface IFetchedListens : IJsonBasedObject {

    /// <summary>The number of listens fetched.</summary>
    int? Count { get; }

    /// <summary>The listens that were fetched.</summary>
    IReadOnlyList<IListen>? Listens { get; }

    /// <summary>Indicates whether this set of listens represents a currently playing track..</summary>
    bool? PlayingNow { get; }

    /// <summary>The timestamp of the newest listen for the user specified by <see cref="User"/>.</summary>
    DateTime? Timestamp { get; }

    /// <summary>The MusicBrainz ID of the user for which the listens were fetched.</summary>
    string? User { get; }

  }

}
