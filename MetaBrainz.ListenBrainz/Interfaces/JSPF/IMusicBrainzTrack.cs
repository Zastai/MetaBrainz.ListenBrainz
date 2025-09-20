using System;
using System.Collections.Generic;

using MetaBrainz.Common.Json;

namespace MetaBrainz.ListenBrainz.Interfaces.JSPF;

/// <summary>Additional information for a track on a playlist, as defined by MusicBrainz.</summary>
/// <seealso href="https://musicbrainz.org/doc/jspf#track"/>
public interface IMusicBrainzTrack : IJsonBasedObject {

  /// <summary>The timestamp for when this track was added to the playlist.</summary>
  DateTimeOffset? Added { get; }

  /// <summary>The ListenBrainz user who added this track.</summary>
  string? AddedBy { get; }

  /// <summary>
  /// Additional playlist metadata that may be used by playlist generation tools. The contents are defined by those playlist
  /// generation tools.
  /// </summary>
  IReadOnlyDictionary<string, object?>? AdditionalMetadata { get; }

  /// <summary>MusicBrainz artist URIs that identify the track's credited artist(s).</summary>
  IReadOnlyList<Uri>? ArtistIds { get; }

  /// <summary>The internal ID for the track's release in the CoverArt Archive.</summary>
  long? CoverArtId { get; }

  /// <summary>The MusicBrainz ID for the track's release in the CoverArt Archive.</summary>
  Guid? CoverArtReleaseId { get; }

  /// <summary>The MusicBrainz credits for the track's artists.</summary>
  IReadOnlyList<IArtistCredit>? Credits { get; }

  /// <summary>The MusicBrainz release URI for the release containing this track.</summary>
  Uri? ReleaseId { get; }

}
