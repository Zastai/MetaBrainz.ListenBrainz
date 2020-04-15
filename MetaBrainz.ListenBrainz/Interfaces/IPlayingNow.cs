using JetBrains.Annotations;

using MetaBrainz.Common.Json;

namespace MetaBrainz.ListenBrainz.Interfaces {

  /// <summary>Information about what a user is currently listening to.</summary>
  [PublicAPI]
  public interface IPlayingNow : IJsonBasedObject {

    /// <summary>
    /// The currently-playing track, or <see langword="null"/> if the user is not currently listening to anything.
    /// </summary>
    IPlayingTrack? Track { get; }

    /// <summary>The MusicBrainz ID of the user for which the currently-playing listen was fetched.</summary>
    string User { get; }

  }

}
