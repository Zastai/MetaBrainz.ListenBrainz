using System;
using System.Collections.Generic;

using JetBrains.Annotations;

namespace MetaBrainz.ListenBrainz.Interfaces {

  /// <summary>
  /// Additional information about the track.<br/>
  /// All fields are stored in <see cref="AllFields"/>, but an attempt has been made to capture a number of the more common fields
  /// (like the ones set by the last.fm import or the Spotify extension) as separate properties.
  /// </summary>
  /// <remarks>
  /// Most of this information is provided by client applications and stored as submitted. As such there is no real guarantee that
  /// any of it is correct.
  /// </remarks>
  [PublicAPI]
  public interface IAdditionalInfo {

    /// <summary>All additional information fields.</summary>
    /// <remarks>Please file a ticket if there's a common field for which you want an explicit property to be added.</remarks>
    IReadOnlyDictionary<string, object?> AllFields { get; }

    /// <summary>The MusicBrainz IDs for the track's artists.</summary>
    IReadOnlyList<Guid?>? ArtistIds { get; }

    /// <summary>The names of the track's artists.</summary>
    IReadOnlyList<string?>? ArtistNames { get; }

    /// <summary>The track's disc number.</summary>
    int? DiscNumber { get; }

    /// <summary>The track's duration.</summary>
    TimeSpan? Duration { get; }

    /// <summary>The MusicBrainz ID for the track's artist, as determined from a Last.fm import (may not be accurate).</summary>
    Guid? ImportedArtistId { get; }

    /// <summary>
    /// The MusicBrainz ID for the release containing the track, as determined from a Last.fm import (may not be accurate).
    /// </summary>
    Guid? ImportedReleaseId { get; }

    /// <summary>The ISRC for the track.</summary>
    string? Isrc { get; }

    /// <summary>The application used to listed to the track.</summary>
    string? ListeningFrom { get; }

    /// <summary>The MessyBrainz ID for the track's artist.</summary>
    Guid? MessyArtistId { get; }

    /// <summary>The MessyBrainz ID for the track's recording.</summary>
    Guid? MessyRecordingId { get; }

    /// <summary>The MessyBrainz ID for the track's release.</summary>
    Guid? MessyReleaseId { get; }

    /// <summary>The MusicBrainz ID for the track's recording.</summary>
    Guid? RecordingId { get; }

    /// <summary>The artist name for the track's release.</summary>
    string? ReleaseArtistName { get; }

    /// <summary>The artist names for the track's release.</summary>
    IReadOnlyList<string?>? ReleaseArtistNames { get; }

    /// <summary>The MusicBrainz ID for the track's release group.</summary>
    Guid? ReleaseGroupId { get; }

    /// <summary>The MusicBrainz ID for the track's release.</summary>
    Guid? ReleaseId { get; }

    /// <summary>The Spotify IDs for the track's album artists.</summary>
    IReadOnlyList<Uri?>? SpotifyAlbumArtistIds { get; }

    /// <summary>The Spotify ID for the track's album.</summary>
    Uri? SpotifyAlbumId { get; }

    /// <summary>The Spotify IDs for the track's artists.</summary>
    IReadOnlyList<Uri?>? SpotifyArtistIds { get; }

    /// <summary>The track's Spotify ID.</summary>
    Uri? SpotifyId { get; }

    /// <summary>The track's tags.</summary>
    IReadOnlyList<string?>? Tags { get; }

    /// <summary>The MusicBrainz ID for the track.</summary>
    Guid? TrackId { get; }

    /// <summary>The track's track number.</summary>
    int? TrackNumber { get; }

    /// <summary>The MusicBrainz IDs of the track's associated works.</summary>
    IReadOnlyList<Guid?>? WorkIds { get; }

  }

}
