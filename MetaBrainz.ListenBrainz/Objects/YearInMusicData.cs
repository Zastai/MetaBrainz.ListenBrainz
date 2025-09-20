﻿using System;
using System.Collections.Generic;

using MetaBrainz.Common.Json;
using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

internal sealed class YearInMusicData : JsonBasedObject, IYearInMusicData {

  public int? ArtistCount { get; init; }

  public IReadOnlyList<IArtistCountryInfo>? ArtistMap { get; init; }

  public string? DayOfWeek { get; init; }

  public int? ListenCount { get; init; }

  public TimeSpan? ListeningTime { get; init; }

  public IReadOnlyList<IListenTimeRange>? ListensPerDay { get; init; }

  public IReadOnlyDictionary<string, int>? MostListenedYear { get; init; }

  public string? MostProminentColor { get; init; }

  public int? NewArtistsDiscovered { get; init; }

  public IReadOnlyList<INewRelease>? NewReleasesOfTopArtists { get; init; }

  public int? RecordingCount { get; init; }

  public int? ReleaseCount { get; init; }

  public int? ReleaseGroupCount { get; init; }

  public IReadOnlyList<ITopArtist>? TopArtists { get; init; }

  public object? TopDiscoveriesPlaylist { get; init; }

  public IReadOnlyDictionary<string, Uri>? TopDiscoveriesPlaylistCoverArt { get; init; }

  public IReadOnlyList<ITopGenre>? TopGenres { get; init; }

  public object? TopMissedRecordingsPlaylist { get; init; }

  public IReadOnlyDictionary<string, Uri>? TopMissedRecordingsPlaylistCoverArt { get; init; }

  public object? TopNewRecordingsPlaylist { get; init; }

  public IReadOnlyDictionary<string, Uri>? TopNewRecordingsPlaylistCoverArt { get; init; }

  public IReadOnlyList<ITopRecording>? TopRecordings { get; init; }

  public object? TopRecordingsPlaylist { get; init; }

  public IReadOnlyDictionary<string, Uri>? TopRecordingsPlaylistCoverArt { get; init; }

  public IReadOnlyDictionary<string, decimal>? SimilarUsers { get; init; }

  public IReadOnlyList<IReleaseGroupInfo>? TopReleaseGroups { get; init; }

  public IReadOnlyList<ITopRelease>? TopReleases { get; init; }

  public IReadOnlyDictionary<string, Uri>? TopReleasesCoverArt { get; init; }

}
