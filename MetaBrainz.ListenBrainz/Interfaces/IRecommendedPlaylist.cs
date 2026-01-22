using System.Collections.Generic;

using MetaBrainz.Common.Json;
using MetaBrainz.ListenBrainz.Interfaces.JSPF;

namespace MetaBrainz.ListenBrainz.Interfaces;

/// <summary>The results of a recommended playlists request.</summary>
public interface IRecommendedPlaylist : IJsonBasedObject {
  /// <summary>The maximum number of recommended playlists returned.</summary>
  int Count { get; }

  /// <summary>The (0-based) offset of the returned recommended playlists from the start of the full set.</summary>
  int Offset { get; }

  /// <summary>The recommended playlists that were found (if any).</summary>
  IReadOnlyList<IPlaylist> Playlists { get; init; }

  /// <summary>The total number of recommended playlists.</summary>
  int TotalCount { get; }
}
