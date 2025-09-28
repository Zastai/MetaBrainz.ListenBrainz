using System.Collections.Generic;

using MetaBrainz.Common.Json;
using MetaBrainz.ListenBrainz.Interfaces;
using MetaBrainz.ListenBrainz.Interfaces.JSPF;

namespace MetaBrainz.ListenBrainz.Objects;

internal sealed class LBRadioPlaylist : JsonBasedObject, ILBRadioPlaylist {

  public required IReadOnlyList<string> Feedback { get; init; }

  public required IPlaylist Playlist { get; init; }

}
