using System.Collections.Generic;

using MetaBrainz.Common.Json;
using MetaBrainz.ListenBrainz.Interfaces.JSPF;

namespace MetaBrainz.ListenBrainz.Interfaces;

/// <summary>The results of a playlist search.</summary>
public interface IFoundPlaylists : IJsonBasedObject {

  /// <summary>The maximum number of matches returned.</summary>
  int Count { get; }

  /// <summary>The (0-based) offset of the returned playlists from the start of the full set.</summary>
  int Offset { get; }

  /// <summary>The playlists that were found (if any).</summary>
  IReadOnlyList<IPlaylist> Playlists { get; init; }

  /// <summary>The total number of matching playlists.</summary>
  int TotalCount { get; }

}
