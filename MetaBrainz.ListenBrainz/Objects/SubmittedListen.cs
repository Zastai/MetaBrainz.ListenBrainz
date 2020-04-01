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
    /// The time at which the track was listened to; <see langword="null"/> for will use the current timestamp.
    /// </param>
    public SubmittedListen(string track, string artist, DateTime? timestamp = null) : base(track, artist) {
      this.Timestamp = timestamp ?? DateTime.UtcNow;
    }

    /// <inheritdoc/>
    public DateTime Timestamp { get; set; }

  }

}
