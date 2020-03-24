using System;
using System.Collections.Generic;

using JetBrains.Annotations;

namespace MetaBrainz.ListenBrainz.Interfaces {

  /// <summary>
  /// Additional information about the track.<br/>
  /// All fields are stored in <see cref="AllFields"/>, but an attempt has been made to capture a number of the more common fields
  /// (like the ones set by the last.fm import or the Spotify extension) as separate properties.
  /// </summary>
  [PublicAPI]
  public interface IAdditionalInfo {

    /// <summary>All additional information fields.</summary>
    /// <remarks>Please file a ticket if there's a common field for which you want an explicit property to be added.</remarks>
    IReadOnlyDictionary<string, object> AllFields { get; }

    /// <summary>The MusicBrainz IDs of the track's artists.</summary>
    public IReadOnlyList<Guid> ArtistIds { get; }

    /// <summary>The names of the track's artists.</summary>
    public IReadOnlyList<string> ArtistNames { get; }

    /// <summary>The track's disc number.</summary>
    public int? DiscNumber { get; }

    /// <summary>The track's duration.</summary>
    public TimeSpan? Duration { get; }

    /// <summary>The ISRC for the track.</summary>
    public string Isrc { get; }

    /// <summary>The application used to listed to the track.</summary>
    public string ListeningFrom { get; }

    /// <summary>The MessyBrainz ID for the track's artist.</summary>
    public Guid? MessyArtistId { get; }

    /// <summary>The MessyBrainz ID for the track's recording.</summary>
    public Guid? MessyRecordingId { get; }

    /// <summary>The MessyBrainz ID for the track's release.</summary>
    public Guid? MessyReleaseId { get; }

    /// <summary>The MusicBrainz ID for the track's recording.</summary>
    public Guid? RecordingId { get; }

    /// <summary>The artist name for the track's release.</summary>
    public string ReleaseArtistName { get; }

    /// <summary>The artist names for the track's release.</summary>
    public IReadOnlyList<string> ReleaseArtistNames { get; }

    /// <summary>The MusicBrainz ID for the track's release group.</summary>
    public Guid? ReleaseGroupId { get; }

    /// <summary>The MusicBrainz ID for the track's release.</summary>
    public Guid? ReleaseId { get; }

    /// <summary>The Spotify IDs for the track's album artists.</summary>
    public IReadOnlyList<Uri> SpotifyAlbumArtistIds { get; }

    /// <summary>The Spotify ID for the track's album.</summary>
    public Uri SpotifyAlbumId { get; }

    /// <summary>The Spotify IDs for the track's artists.</summary>
    public IReadOnlyList<Uri> SpotifyArtistIds { get; }

    /// <summary>The track's Spotify ID.</summary>
    public Uri SpotifyId { get; }

    /// <summary>The track's tags.</summary>
    public IReadOnlyList<string> Tags { get; }

    /// <summary>The MusicBrainz ID for the track.</summary>
    public Guid? TrackId { get; }

    /// <summary>The track's track number.</summary>
    public int? TrackNumber { get; }

    /// <summary>The MusicBrainz IDs of the track's associated works.</summary>
    public IReadOnlyList<Guid> WorkIds { get; }

  }

}
