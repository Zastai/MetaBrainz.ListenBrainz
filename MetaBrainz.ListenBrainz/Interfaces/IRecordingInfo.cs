using System;
using System.Collections.Generic;

using JetBrains.Annotations;

namespace MetaBrainz.ListenBrainz.Interfaces;

/// <summary>Statistical information about a recording.</summary>
[PublicAPI]
public interface IRecordingInfo {

  /// <summary>The MusicBrainz IDs for the recording's artist(s), if available.</summary>
  IReadOnlyList<Guid>? ArtistIds { get; }

  /// <summary>The MessyBrainz ID for the recording's artist, if available.</summary>
  Guid? ArtistMessyId { get; }

  /// <summary>The recording's artist's name, if available.</summary>
  string? ArtistName { get; }

  /// <summary>The internal ID for the recording's release in the CoverArt Archive.</summary>
  long? CoverArtId { get; }

  /// <summary>The MusicBrainz ID for the recording's release in the CoverArt Archive.</summary>
  Guid? CoverArtReleaseId { get; }

  /// <summary>The MusicBrainz credits for the recording's artists.</summary>
  IReadOnlyList<IArtistCredit>? Credits { get; }

  /// <summary>The number of times the recording was listened to.</summary>
  int ListenCount { get; }

  /// <summary>The MusicBrainz ID for the recording, if available.</summary>
  Guid? Id { get; }

  /// <summary>The MessyBrainz ID for the recording, if available.</summary>
  Guid? MessyId { get; }

  /// <summary>The recording's name.</summary>
  string Name { get; }

  /// <summary>The MusicBrainz IDs for the recording's release, if available.</summary>
  Guid? ReleaseId { get; }

  /// <summary>The MessyBrainz ID for the recording's release, if available.</summary>
  Guid? ReleaseMessyId { get; }

  /// <summary>The recording's release's name, if available.</summary>
  string? ReleaseName { get; }

}
