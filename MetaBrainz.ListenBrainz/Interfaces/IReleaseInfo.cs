using System;
using System.Collections.Generic;

using JetBrains.Annotations;

namespace MetaBrainz.ListenBrainz.Interfaces;

/// <summary>Statistical information about a release.</summary>
[PublicAPI]
public interface IReleaseInfo {

  /// <summary>The MusicBrainz IDs for the release's artist(s).</summary>
  IReadOnlyList<Guid>? ArtistIds { get; }

  /// <summary>The MessyBrainz ID for the release's artist.</summary>
  Guid? ArtistMessyId { get; }

  /// <summary>The release's artist's name.</summary>
  string? ArtistName { get; }

  /// <summary>The internal ID for the release in the CoverArt Archive.</summary>
  long? CoverArtId { get; }

  /// <summary>The MusicBrainz ID for the release in the CoverArt Archive.</summary>
  Guid? CoverArtReleaseId { get; }

  /// <summary>The MusicBrainz credits for the release's artists.</summary>
  IReadOnlyList<IArtistCredit>? Credits { get; }

  /// <summary>The MusicBrainz ID for the release.</summary>
  Guid? Id { get; }

  /// <summary>The number of times the release's tracks were listened to.</summary>
  int ListenCount { get; }

  /// <summary>The MessyBrainz ID for the release.</summary>
  Guid? MessyId { get; }

  /// <summary>The release's name.</summary>
  string Name { get; }

}
