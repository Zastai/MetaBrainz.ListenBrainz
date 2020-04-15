using JetBrains.Annotations;

using MetaBrainz.Common.Json;

namespace MetaBrainz.ListenBrainz.Interfaces {

  /// <summary>A currently-playing track.</summary>
  [PublicAPI]
  public interface IPlayingTrack : IJsonBasedObject {

    /// <summary>Information about the track.</summary>
    ITrackInfo Info { get; }

  }

}
