using System;
using System.Collections.Generic;

namespace MetaBrainz.ListenBrainz.Interfaces;

/// <summary>Information about a release group's top listeners.</summary>
public interface IReleaseGroupListeners : IListenerInfo, IStatistics {

  /// <summary>The MusicBrainz IDs for the release group's artist(s).</summary>
  IReadOnlyList<Guid>? ArtistIds { get; }

  /// <summary>The release group's artist's name.</summary>
  string? ArtistName { get; }

  /// <summary>The internal ID for the release group in the CoverArt Archive.</summary>
  long? CoverArtId { get; }

  /// <summary>The MusicBrainz ID for the release group in the CoverArt Archive.</summary>
  Guid? CoverArtReleaseGroupId { get; }

  /// <summary>The MusicBrainz ID for the release group.</summary>
  Guid Id { get; }

  /// <summary>The release group's name.</summary>
  string Name { get; }

}
