using System;
using System.Collections.Generic;

namespace MetaBrainz.ListenBrainz.Interfaces.JSPF;

/// <summary>Information about a MusicBrainz recording, associated with a track on a playlist.</summary>
public interface IMusicBrainzRecording {

  /// <summary>The MusicBrainz IDs for the recording's artist(s).</summary>
  IReadOnlyList<Guid>? ArtistIds { get; }

}
