using System;

using JetBrains.Annotations;

using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects {

  /// <inheritdoc cref="ISubmittedListen"/>
  [PublicAPI]
  public class SubmittedListen : SubmittedListenData, ISubmittedListen {

    /// <summary>Creates a new listen.</summary>
    /// <param name="track">The listened track's name.</param>
    /// <param name="artist">The listened track's artist.</param>
    /// <param name="timestamp">
    /// The date and time at which the track was listened to; when not specified or <see langword="null"/>, the current UTC date and
    /// time is used.
    /// </param>
    public SubmittedListen(string track, string artist, DateTimeOffset? timestamp = null) : base(track, artist) {
      this.Timestamp = timestamp ?? DateTimeOffset.UtcNow;
    }

    /// <summary>Creates a new listen.</summary>
    /// <param name="track">The listened track's name.</param>
    /// <param name="artist">The listened track's artist.</param>
    /// <param name="timestamp">
    /// The date and time at which the track was listened to, specified as the number of seconds since
    /// <see cref="UnixTime.Epoch">the Unix time epoch</see>.
    /// </param>
    public SubmittedListen(string track, string artist, long timestamp) : base(track, artist) {
      this.Timestamp = UnixTime.Convert(timestamp);
    }

    /// <inheritdoc/>
    public DateTimeOffset Timestamp { get; set; }

  }

}
