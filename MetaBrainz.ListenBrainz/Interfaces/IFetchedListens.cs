using System;
using System.Collections.Generic;

using JetBrains.Annotations;

using MetaBrainz.Common;
using MetaBrainz.Common.Json;

namespace MetaBrainz.ListenBrainz.Interfaces;

/// <summary>Information about a set of fetched listens.</summary>
[PublicAPI]
public interface IFetchedListens : IJsonBasedObject {

  /// <summary>The listens that were fetched.</summary>
  IReadOnlyList<IListen> Listens { get; }

  /// <summary>The timestamp of the newest listen recorded for the user.</summary>
  /// <remarks>
  /// This looks at the entire dataset, not just the date range that may have been specified in the request. The listen(s) with this
  /// timestamp will not necessarily be included in <see cref="Listens"/>.
  /// </remarks>
  DateTimeOffset Newest { get; }

  /// <summary>The timestamp of the oldest listen recorded for the user.</summary>
  /// <remarks>
  /// This looks at the entire dataset, not just the date range that may have been specified in the request. The listen(s) with this
  /// timestamp will not necessarily be included in <see cref="Listens"/>.
  /// </remarks>
  DateTimeOffset Oldest { get; }

  /// <summary>The MusicBrainz ID of the user for which the listens were fetched.</summary>
  string User { get; }

}
