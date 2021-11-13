using MetaBrainz.Common.Json;
using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

internal sealed class PlayingTrack : JsonBasedObject, IPlayingTrack {

  public PlayingTrack(ITrackInfo info) {
    this.Info = info;
  }

  public ITrackInfo Info { get; }

}
