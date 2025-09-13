using MetaBrainz.Common.Json;
using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

internal sealed class PlayingTrack : JsonBasedObject, IPlayingTrack {

  public required ITrackInfo Info { get; init; }

}
