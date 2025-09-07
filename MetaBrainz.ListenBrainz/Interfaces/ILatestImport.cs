using System;

using JetBrains.Annotations;

using MetaBrainz.Common;
using MetaBrainz.Common.Json;

namespace MetaBrainz.ListenBrainz.Interfaces;

/// <summary>Information about a user's most recent import of listen data.</summary>
[PublicAPI]
public interface ILatestImport : IJsonBasedObject {

  /// <summary>The timestamp of the newest listen submitted in previous imports.</summary>
  DateTimeOffset? Timestamp { get; }

  /// <summary>
  /// The timestamp of the newest listen submitted in previous imports, expressed as the number of seconds since
  /// <see cref="DateTimeOffset.UnixEpoch">the Unix time epoch</see>.
  /// </summary>
  long? UnixTimestamp { get; }

  /// <summary>The MusicBrainz ID of the user.</summary>
  string? User { get; }

}
