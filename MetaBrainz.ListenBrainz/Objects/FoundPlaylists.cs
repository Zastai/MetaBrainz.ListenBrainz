using System.Collections.Generic;

using MetaBrainz.Common.Json;
using MetaBrainz.ListenBrainz.Interfaces;
using MetaBrainz.ListenBrainz.Interfaces.JSPF;

namespace MetaBrainz.ListenBrainz.Objects;

internal sealed class FoundPlaylists : JsonBasedObject, IFoundPlaylists {

  public required int Count { get; init; }

  public required int Offset { get; init; }

  public required IReadOnlyList<IPlaylist> Playlists { get; init; }

  public required int TotalCount { get; init; }

}
