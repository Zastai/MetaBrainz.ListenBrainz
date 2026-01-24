using System;
using System.Collections.Generic;

using MetaBrainz.Common.Json;
using MetaBrainz.ListenBrainz.Interfaces.JSPF;

namespace MetaBrainz.ListenBrainz.Interfaces;

/// <summary>"Year in Music" data.</summary>
/// <remarks>This data is not currently documented, so the field documentation is provisional.</remarks>
public interface IYearInMusicData : IJsonBasedObject {

  /// <summary>The total number of (distinct) artists listened to over the course of the year.</summary>
  /// <remarks>This field is only provided for the Year in Music 2022 and later.</remarks>
  int? ArtistCount { get; }

  /// <summary>Information about artist listen counts, grouped by country.</summary>
  /// <remarks>This field is only provided for the Year in Music 2022 and later.</remarks>
  IReadOnlyList<IArtistCountryInfo>? ArtistMap { get; }

  /// <summary>The weekday that sees the most listens recorded.</summary>
  string? DayOfWeek { get; }

  /// <summary>The total number of listens that were recorded over the course of the year.</summary>
  int? ListenCount { get; }

  /// <summary>The total amount of recorded listening time over the course of the year.</summary>
  /// <remarks>This field is only provided for the Year in Music 2022 and later.</remarks>
  TimeSpan? ListeningTime { get; }

  /// <summary>The number of listens recorded on each day of the year.</summary>
  IReadOnlyList<IListenTimeRange>? ListensPerDay { get; }

  /// <summary>
  /// Information about the number of listens for recordings released in a particular year.<br/>
  /// The key is the release year, the value is the listen count.
  /// </summary>
  IReadOnlyDictionary<string, int>? MostListenedYear { get; }

  /// <summary>The most prominent color on cover art associated with things the user listened to.</summary>
  /// <remarks>This field is only provided for the Year in Music 2021.</remarks>
  string? MostProminentColor { get; }

  /// <summary>The total number of (distinct) new artists listened to over the course of the year.</summary>
  /// <remarks>This field is only provided for the Year in Music 2022 and later.</remarks>
  int? NewArtistsDiscovered { get; }

  /// <summary>Information about new releases from the user's top artists.</summary>
  IReadOnlyList<INewRelease>? NewReleasesOfTopArtists { get; }

  /// <summary>The total number of (distinct) recordings (tracks) listened to over the course of the year.</summary>
  /// <remarks>This field is only provided for the Year in Music 2022 and later.</remarks>
  int? RecordingCount { get; }

  /// <summary>The total number of (distinct) releases (albums) listened to over the course of the year.</summary>
  /// <remarks>This field is only provided for the Year in Music 2022.</remarks>
  int? ReleaseCount { get; }

  /// <summary>The total number of (distinct) release groups (albums) listened to over the course of the year.</summary>
  /// <remarks>This field is only provided for the Year in Music 2023 and later.</remarks>
  int? ReleaseGroupCount { get; }

  /// <summary>Users with tastes similar to this one.</summary>
  IReadOnlySet<ISimilarUser>? SimilarUsers { get; }

  /// <summary>Information about a user's top artists.</summary>
  IReadOnlyList<ITopArtist>? TopArtists { get; }

  /// <summary>Playlist for the "top discoveries for this year".</summary>
  IPlaylist? TopDiscoveriesPlaylist { get; }

  /// <summary>
  /// Mappings to CAA URLs for the recordings in <see cref="TopDiscoveriesPlaylist"/>.<br/>
  /// The key is the MusicBrainz ID for the recording, the value is the URL to an image from the CoverArt Archive.
  /// </summary>
  /// <remarks>This field is only provided for the Year in Music 2021 and 2022.</remarks>
  IReadOnlyDictionary<string, Uri>? TopDiscoveriesPlaylistCoverArt { get; }

  /// <summary>Information about a user's top genres.</summary>
  /// <remarks>This field is only provided for the Year in Music 2023 and later.</remarks>
  IReadOnlyList<ITopGenre>? TopGenres { get; }

  /// <summary>Playlist for the "top missed recordings for this year".</summary>
  IPlaylist? TopMissedRecordingsPlaylist { get; }

  /// <summary>
  /// Mappings to CAA URLs for the recordings in <see cref="TopMissedRecordingsPlaylist"/>.<br/>
  /// The key is the MusicBrainz ID for the recording, the value is the URL to an image from the CoverArt Archive.
  /// </summary>
  /// <remarks>This field is only provided for the Year in Music 2021 and 2022.</remarks>
  IReadOnlyDictionary<string, Uri>? TopMissedRecordingsPlaylistCoverArt { get; }

  /// <summary>Playlist for the "top new recordings for this year".</summary>
  /// <remarks>This field is only provided for the Year in Music 2021.</remarks>
  IPlaylist? TopNewRecordingsPlaylist { get; }

  /// <summary>
  /// Mappings to CAA URLs for the recordings in <see cref="TopNewRecordingsPlaylist"/>.<br/>
  /// The key is the MusicBrainz ID for the recording, the value is the URL to an image from the CoverArt Archive.
  /// </summary>
  /// <remarks>This field is only provided for the Year in Music 2021.</remarks>
  IReadOnlyDictionary<string, Uri>? TopNewRecordingsPlaylistCoverArt { get; }

  /// <summary>Information about a user's top recordings (tracks).</summary>
  IReadOnlyList<ITopRecording>? TopRecordings { get; }

  /// <summary>Playlist for the "top recordings for this year".</summary>
  /// <remarks>This field is only provided for the Year in Music 2021.</remarks>
  IPlaylist? TopRecordingsPlaylist { get; }

  /// <summary>
  /// Mappings to CAA URLs for the recordings in <see cref="TopRecordingsPlaylist"/>.<br/>
  /// The key is the MusicBrainz ID for the recording, the value is the URL to an image from the CoverArt Archive.
  /// </summary>
  /// <remarks>This field is only provided for the Year in Music 2021.</remarks>
  IReadOnlyDictionary<string, Uri>? TopRecordingsPlaylistCoverArt { get; }

  /// <summary>Information about a user's top release groups (albums).</summary>
  /// <remarks>This field is only provided for the Year in Music 2023 and later.</remarks>
  IReadOnlyList<IReleaseGroupInfo>? TopReleaseGroups { get; }

  /// <summary>Information about a user's top releases (albums).</summary>
  /// <remarks>This field is only provided for the Year in Music 2021 and 2022.</remarks>
  IReadOnlyList<ITopRelease>? TopReleases { get; }

  /// <summary>
  /// Mappings to CAA URLs for the releases in <see cref="TopReleases"/>.<br/>
  /// The key is the MusicBrainz ID for the release (as found in <see cref="TopReleases"/>), the value is the URL to an image from
  /// the CoverArt Archive.
  /// </summary>
  /// <remarks>This field is only provided for the Year in Music 2021.</remarks>
  IReadOnlyDictionary<string, Uri>? TopReleasesCoverArt { get; }

}
