using System;

using JetBrains.Annotations;

using MetaBrainz.Common;
using MetaBrainz.Common.Json;

namespace MetaBrainz.ListenBrainz.Interfaces;

/// <summary>Information about a single listen.</summary>
[PublicAPI]
public interface IListen : IJsonBasedObject {

  /// <summary>The timestamp at which the listen information was inserted in the database, in human-readable format.</summary>
  string InsertedAt { get; }

  /// <summary>The MessyBrainz ID for the recording that was listened to.</summary>
  Guid MessyRecordingId { get; }

  /// <summary>The timestamp at which the listen occurred.</summary>
  DateTimeOffset Timestamp { get; }

  /// <summary>Information about the track that was listened to.</summary>
  ITrackInfo Track { get; }

  /// <summary>
  /// The timestamp for the listen, expressed as the number of seconds since <see cref="UnixTime.Epoch">the Unix time epoch</see>.
  /// </summary>
  long UnixTimestamp { get; }

  /// <summary>The MusicBrainz ID of the user who submitted the listen.</summary>
  string User { get; }

}
