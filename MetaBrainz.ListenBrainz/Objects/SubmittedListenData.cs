using JetBrains.Annotations;

using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects {

  /// <inheritdoc cref="ISubmittedListenData"/>
  [PublicAPI]
  public class SubmittedListenData : ISubmittedListenData {

    /// <summary>Creates new listen data.</summary>
    /// <param name="track">The listened track's name.</param>
    /// <param name="artist">The listened track's artist.</param>
    public SubmittedListenData(string track, string artist) {
      this.Track = new SubmittedTrackInfo(track, artist);
    }

    /// <inheritdoc cref="ISubmittedListenData.Track"/>
    public readonly SubmittedTrackInfo Track;

    ISubmittedTrackInfo ISubmittedListenData.Track => this.Track;

  }

}
