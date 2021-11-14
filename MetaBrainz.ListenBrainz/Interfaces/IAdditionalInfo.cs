using System;
using System.Collections.Generic;

using JetBrains.Annotations;

namespace MetaBrainz.ListenBrainz.Interfaces;

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

  /// <summary>The name of the program used to listen to the track; this should not include a version number.</summary>
  string? MediaPlayer { get; }

  /// <summary>The version of the program used to listen to the track.</summary>
  string? MediaPlayerVersion { get; }

  /// <summary>The MessyBrainz ID for the track's artist.</summary>
  Guid? MessyArtistId { get; }

  /// <summary>The MessyBrainz ID for the track's recording.</summary>
  Guid? MessyRecordingId { get; }

  /// <summary>The MessyBrainz ID for the track's release.</summary>
  Guid? MessyReleaseId { get; }

  /// <summary>The MusicBrainz ID for the track's recording.</summary>
  Guid? RecordingId { get; }

  /// <summary>
  /// If the track comes from an online service, the canonical domain of this service (so "spotify.com", not something like
  /// "http://open.spotify.com").
  /// </summary>
  string? MusicService { get; }

  /// <summary>
  /// If the track comes from an online service, a name that represents the service. Only relevant when <see cref="MusicService"/>
  /// is not set.
  /// </summary>
  string? MusicServiceName { get; }

  /// <summary>
  /// If the song of this listen comes from an online source, the URL to the place where it was available at the time.<br/>
  /// This could be a spotify url (see <see cref="SpotifyId"/>), a YouTube video URL, a Soundcloud recording page URL, or the full
  /// URL to a public MP3 file. If there is a webpage for this song (e.g. YouTube page, Soundcloud page), that should be used here
  /// instead of the URL to an actual audio resource.
  /// </summary>
  Uri? OriginUrl { get; }

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

  /// <summary>
  /// The name of the client used to submit the listen to ListenBrainz; this should not include a version number.<br/>
  /// If the media player has the ability to submit listens built in, then this value may be the same as <see cref="MediaPlayer"/>.
  /// </summary>
  string? SubmissionClient { get; }

  /// <summary>The version of the client used to submit the listen to ListenBrainz.</summary>
  string? SubmissionClientVersion { get; }

  /// <summary>The track's tags.</summary>
  IReadOnlyList<string?>? Tags { get; }

  /// <summary>The MusicBrainz ID for the track.</summary>
  Guid? TrackId { get; }

  /// <summary>The track's track number.</summary>
  int? TrackNumber { get; }

  /// <summary>The MusicBrainz IDs of the track's associated works.</summary>
  IReadOnlyList<Guid?>? WorkIds { get; }

}
