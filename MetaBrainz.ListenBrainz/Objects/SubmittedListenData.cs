using JetBrains.Annotations;

using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

/// <inheritdoc cref="ISubmittedListenData"/>
[PublicAPI]
public class SubmittedListenData : ISubmittedListenData {

  /// <summary>Creates new listen data.</summary>
  /// <param name="track">The listened track's name.</param>
  /// <param name="artist">The listened track's artist.</param>
  /// <param name="release">The listened track's release.</param>
  public SubmittedListenData(string track, string artist, string? release = null) {
    this.Track = new SubmittedTrackInfo(track, artist, release);
  }

  /// <inheritdoc cref="ISubmittedListenData.Track"/>
  public readonly SubmittedTrackInfo Track;

  ISubmittedTrackInfo ISubmittedListenData.Track => this.Track;

}
