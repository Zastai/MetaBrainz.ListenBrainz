using System;

using JetBrains.Annotations;

using MetaBrainz.Common;
using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

/// <inheritdoc cref="ISubmittedListen"/>
[PublicAPI]
public class SubmittedListen : SubmittedListenData, ISubmittedListen {

  /// <summary>Creates a new listen.</summary>
  /// <param name="timestamp">The date and time at which the track was listened to.</param>
  /// <param name="track">The listened track's name.</param>
  /// <param name="artist">The listened track's artist.</param>
  /// <param name="release">The listened track's release.</param>
  public SubmittedListen(DateTimeOffset timestamp, string track, string artist, string? release = null)
  : base(track, artist, release) {
    this.Timestamp = timestamp;
  }

  /// <summary>Creates a new listen.</summary>
  /// <param name="timestamp">
  /// The date and time at which the track was listened to, expressed as the number of seconds since
  /// <see cref="UnixTime.Epoch">the Unix time epoch</see>.
  /// </param>
  /// <param name="track">The listened track's name.</param>
  /// <param name="artist">The listened track's artist.</param>
  /// <param name="release">The listened track's release.</param>
  public SubmittedListen(long timestamp, string track, string artist, string? release = null) : base(track, artist, release) {
    this.Timestamp = UnixTime.Convert(timestamp);
  }

  /// <summary>Creates a new listen, using the current (UTC) date and time as timestamp.</summary>
  /// <param name="track">The listened track's name.</param>
  /// <param name="artist">The listened track's artist.</param>
  /// <param name="release">The listened track's release.</param>
  public SubmittedListen(string track, string artist, string? release = null)
  : this(DateTimeOffset.UtcNow, track, artist, release) {
  }

  /// <inheritdoc/>
  public DateTimeOffset Timestamp { get; set; }

}
