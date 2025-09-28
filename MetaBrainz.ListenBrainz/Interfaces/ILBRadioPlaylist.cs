using System.Collections.Generic;

using MetaBrainz.Common.Json;
using MetaBrainz.ListenBrainz.Interfaces.JSPF;

namespace MetaBrainz.ListenBrainz.Interfaces;

/// <summary>An LB Radio recommendation playlist based on a user-supplied prompt.</summary>
public interface ILBRadioPlaylist : IJsonBasedObject {

  /// <summary>Feedback messages provided by the playlist generation.</summary>
  IReadOnlyList<string> Feedback { get; }

  /// <summary>The generated playlist.</summary>
  IPlaylist Playlist { get; }

}
