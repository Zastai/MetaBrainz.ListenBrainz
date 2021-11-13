using System;
using System.Collections.Generic;

using JetBrains.Annotations;

using MetaBrainz.Common.Json;

namespace MetaBrainz.ListenBrainz.Interfaces;

/// <summary>Information about a set of fetched listens.</summary>
[PublicAPI]
public interface IFetchedListens : IJsonBasedObject {

  /// <summary>The listens that were fetched.</summary>
  IReadOnlyList<IListen> Listens { get; }

  /// <summary>The timestamp of the newest listen included in <see cref="Listens"/>.</summary>
  DateTimeOffset Timestamp { get; }

  /// <summary>
  /// The timestamp of the newest listen included in <see cref="Listens"/>, expressed as the number of seconds since
  /// <see cref="UnixTime.Epoch">the Unix time epoch</see>.
  /// </summary>
  long UnixTimestamp { get; }

  /// <summary>The MusicBrainz ID of the user for which the listens were fetched.</summary>
  string User { get; }

}
