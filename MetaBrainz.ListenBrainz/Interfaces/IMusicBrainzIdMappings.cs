using System;
using System.Collections.Generic;

using JetBrains.Annotations;

namespace MetaBrainz.ListenBrainz.Interfaces;

/// <summary>
/// Mappings to MusicBrainz IDs as determined by ListenBrainz.<br/>
/// There are similar fields in <see cref="IAdditionalInfo"/>, but those are values supplied at submission time by the client.
/// </summary>
[PublicAPI]
public interface IMusicBrainzIdMappings {

  /// <summary>The MusicBrainz IDs for the track's artists.</summary>
  IReadOnlyList<Guid>? ArtistIds { get; }

  /// <summary>The internal ID for the track in the CoverArt Archive.</summary>
  long? CoverArtId { get; }

  /// <summary>The MusicBrainz release ID for the track in the CoverArt Archive.</summary>
  Guid? CoverArtReleaseId { get; }

  /// <summary>The MusicBrainz credits for the track's artists.</summary>
  IReadOnlyList<IArtistCredit>? Credits { get; }

  /// <summary>The MusicBrainz ID for the track's recording.</summary>
  Guid? RecordingId { get; }

  /// <summary>The name for the track's recording in the MusicBrainz database.</summary>
  string? RecordingName { get; }

  /// <summary>The MusicBrainz ID for the track's release.</summary>
  Guid? ReleaseId { get; }

}
