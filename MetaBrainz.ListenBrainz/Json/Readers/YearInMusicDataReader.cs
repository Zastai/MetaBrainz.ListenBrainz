using System;
using System.Collections.Generic;
using System.Text.Json;

using MetaBrainz.Common.Json;
using MetaBrainz.Common.Json.Converters;
using MetaBrainz.ListenBrainz.Interfaces;
using MetaBrainz.ListenBrainz.Objects;

namespace MetaBrainz.ListenBrainz.Json.Readers;

internal sealed class YearInMusicDataReader : ObjectReader<YearInMusicData> {

  public static readonly YearInMusicDataReader Instance = new();

  protected override YearInMusicData ReadObjectContents(ref Utf8JsonReader reader, JsonSerializerOptions options) {
    int? artistCount = null;
    IReadOnlyList<IArtistCountryInfo>? artistMap = null;
    string? dayOfWeek = null;
    IReadOnlyDictionary<string, Uri>? discoveriesCoverArt = null;
    object? discoveriesPlaylist = null;
    int? listenCount = null;
    double? listenedMinutes = null;
    IReadOnlyList<IListenTimeRange>? listensPerDay = null;
    IReadOnlyDictionary<string, Uri>? missedRecordingsCoverArt = null;
    object? missedRecordingsPlaylist = null;
    IReadOnlyDictionary<string, int>? mostListenedYear = null;
    string? mostProminentColor = null;
    int? newArtistsDiscovered = null;
    IReadOnlyDictionary<string, Uri>? newRecordingsCoverArt = null;
    object? newRecordingsPlaylist = null;
    IReadOnlyList<INewRelease>? newReleasesOfTopArtists = null;
    int? recordingCount = null;
    IReadOnlyDictionary<string, Uri>? recordingsCoverArt = null;
    object? recordingsPlaylist = null;
    int? releaseCount = null;
    int? releaseGroupCount = null;
    IReadOnlyDictionary<string, decimal>? similarUsers = null;
    IReadOnlyList<ITopArtist>? topArtists = null;
    IReadOnlyList<ITopGenre>? topGenres = null;
    IReadOnlyList<ITopRecording>? topRecordings = null;
    IReadOnlyList<IReleaseGroupInfo>? topReleaseGroups = null;
    IReadOnlyList<ITopRelease>? topReleases = null;
    IReadOnlyDictionary<string, Uri>? topReleasesCoverArt = null;
    Dictionary<string, object?>? rest = null;
    while (reader.TokenType == JsonTokenType.PropertyName) {
      var prop = reader.GetPropertyName();
      try {
        reader.Read();
        switch (prop) {
          case "artist_map":
            artistMap = reader.ReadList(ArtistCountryInfoReader.Instance, options);
            break;
          case "day_of_week":
            dayOfWeek = reader.GetString();
            break;
          case "listens_per_day":
            listensPerDay = reader.ReadList(ListenTimeRangeReader.Instance, options);
            break;
          case "most_listened_year":
            mostListenedYear = reader.ReadDictionary<int>(options);
            break;
          case "most_prominent_color":
            mostProminentColor = reader.GetString();
            break;
          case "new_releases_of_top_artists":
            newReleasesOfTopArtists = reader.ReadList(NewReleaseReader.Instance, options);
            break;
          case "playlist-top-discoveries-for-year":
            discoveriesPlaylist = reader.GetOptionalObject(options);
            break;
          case "playlist-top-discoveries-for-year-coverart":
            discoveriesCoverArt = reader.ReadDictionary<Uri>(options);
            break;
          case "playlist-top-missed-recordings-for-year":
            missedRecordingsPlaylist = reader.GetOptionalObject(options);
            break;
          case "playlist-top-missed-recordings-for-year-coverart":
            missedRecordingsCoverArt = reader.ReadDictionary<Uri>(options);
            break;
          case "playlist-top-new-recordings-for-year":
            newRecordingsPlaylist = reader.GetOptionalObject(options);
            break;
          case "playlist-top-new-recordings-for-year-coverart":
            newRecordingsCoverArt = reader.ReadDictionary<Uri>(options);
            break;
          case "playlist-top-recordings-for-year":
            recordingsPlaylist = reader.GetOptionalObject(options);
            break;
          case "playlist-top-recordings-for-year-coverart":
            recordingsCoverArt = reader.ReadDictionary<Uri>(options);
            break;
          case "similar_users":
            similarUsers = reader.ReadDictionary<decimal>(options);
            break;
          case "top_artists":
            topArtists = reader.ReadList(TopArtistReader.Instance, options);
            break;
          case "top_genres":
            topGenres = reader.ReadList(TopGenreReader.Instance, options);
            break;
          case "top_recordings":
            topRecordings = reader.ReadList(TopRecordingReader.Instance, options);
            break;
          case "top_release_groups":
            topReleaseGroups = reader.ReadList(ReleaseGroupInfoReader.Instance, options);
            break;
          case "top_releases":
            topReleases = reader.ReadList(TopReleaseReader.Instance, options);
            break;
          case "top_releases_coverart":
            topReleasesCoverArt = reader.ReadDictionary<Uri>(options);
            break;
          case "total_artists_count":
            artistCount = reader.GetInt32();
            break;
          case "total_listen_count":
            listenCount = reader.GetInt32();
            break;
          case "total_listening_time":
            listenedMinutes = reader.GetDouble();
            break;
          case "total_new_artists_discovered":
            newArtistsDiscovered = reader.GetInt32();
            break;
          case "total_recordings_count":
            recordingCount = reader.GetInt32();
            break;
          case "total_releases_count":
            releaseCount = reader.GetInt32();
            break;
          case "total_release_groups_count":
            releaseGroupCount = reader.GetInt32();
            break;
          case "yim_artist_map":
            // this is identical to artist_map, and typically huge, so just skip it
            reader.Skip();
            break;
          default:
            rest ??= new Dictionary<string, object?>();
            rest[prop] = reader.GetOptionalObject(options);
            break;
        }
      }
      catch (Exception e) {
        throw new JsonException($"Failed to deserialize the '{prop}' property.", e);
      }
      reader.Read();
    }
    TimeSpan? listeningTime = listenedMinutes is null ? null : TimeSpan.FromSeconds(listenedMinutes.Value);
    return new YearInMusicData {
      ArtistCount = artistCount,
      ArtistMap = artistMap,
      DayOfWeek = dayOfWeek,
      ListenCount = listenCount,
      ListeningTime = listeningTime,
      ListensPerDay = listensPerDay,
      MostListenedYear = mostListenedYear,
      MostProminentColor = mostProminentColor,
      NewArtistsDiscovered = newArtistsDiscovered,
      NewReleasesOfTopArtists = newReleasesOfTopArtists,
      RecordingCount = recordingCount,
      ReleaseCount = releaseCount,
      ReleaseGroupCount = releaseGroupCount,
      SimilarUsers = similarUsers,
      TopArtists = topArtists,
      TopGenres = topGenres,
      TopDiscoveriesPlaylist = discoveriesPlaylist,
      TopDiscoveriesPlaylistCoverArt = discoveriesCoverArt,
      TopMissedRecordingsPlaylist = missedRecordingsPlaylist,
      TopMissedRecordingsPlaylistCoverArt = missedRecordingsCoverArt,
      TopNewRecordingsPlaylist = newRecordingsPlaylist,
      TopNewRecordingsPlaylistCoverArt = newRecordingsCoverArt,
      TopRecordings = topRecordings,
      TopRecordingsPlaylist = recordingsPlaylist,
      TopRecordingsPlaylistCoverArt = recordingsCoverArt,
      TopReleaseGroups = topReleaseGroups,
      TopReleases = topReleases,
      TopReleasesCoverArt = topReleasesCoverArt,
      UnhandledProperties = rest,
    };
  }

}
