using System;

using JetBrains.Annotations;

namespace MetaBrainz.ListenBrainz.Interfaces;

/// <summary>Information about a single listen, including a timestamp.</summary>
[PublicAPI]
public interface ISubmittedListen : ISubmittedListenData {

  /// <summary>The timestamp for the listen.</summary>
  DateTimeOffset Timestamp { get; }

  /// <summary>
  /// The timestamp for the listen, expressed as the number of seconds since
  /// <see cref="DateTimeOffset.UnixEpoch">the Unix time epoch</see>.
  /// </summary>
  long UnixTimestamp { get; }

}
