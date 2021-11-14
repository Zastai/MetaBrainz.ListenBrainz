using System;
using System.Collections.Generic;

namespace MetaBrainz.ListenBrainz.Interfaces;

/// <summary>
/// Mappings to MusicBrainz IDs as determined by ListenBrainz.<br/>
/// There are similar fields in <see cref="IAdditionalInfo"/>, but those are values supplied at submission time by the client.
/// </summary>
public interface IMusicBrainzIdMappings {

  /// <summary>The MusicBrainz IDs for the track's artists.</summary>
  IReadOnlyList<Guid>? ArtistIds { get; }

  /// <summary>The MusicBrainz ID for the track's recording.</summary>
  Guid? RecordingId { get; }

  /// <summary>The MusicBrainz ID for the track's release.</summary>
  Guid? ReleaseId { get; }

}
