using System;
using System.Collections.Generic;

using JetBrains.Annotations;

using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
internal sealed class AdditionalInfo : IAdditionalInfo {

  public AdditionalInfo(Dictionary<string, object?> fields) {
    // Store the full set of fields
    this.AllFields = fields;
    // Extract "simple" well-known fields
    this.ArtistNames = AdditionalInfo.GetObjectList<string>(fields, "artist_names");
    this.ArtistIds = AdditionalInfo.GetValueList<Guid>(fields, "artist_mbids");
    this.DiscNumber = AdditionalInfo.GetValue<int>(fields, "discnumber");
    this.ImportedArtistId = AdditionalInfo.GetValue<Guid>(fields, "lastfm_artist_mbid");
    this.Isrc = AdditionalInfo.GetObject<string>(fields, "isrc");
    this.ImportedReleaseId = AdditionalInfo.GetValue<Guid>(fields, "lastfm_release_mbid");
    this.ListeningFrom = AdditionalInfo.GetObject<string>(fields, "listening_from");
    this.MediaPlayer = AdditionalInfo.GetObject<string>(fields, "media_player");
    this.MediaPlayerVersion = AdditionalInfo.GetObject<string>(fields, "media_player_version");
    this.MessyArtistId = AdditionalInfo.GetValue<Guid>(fields, "artist_msid");
    this.MessyRecordingId = AdditionalInfo.GetValue<Guid>(fields, "recording_msid");
    this.MessyReleaseId = AdditionalInfo.GetValue<Guid>(fields, "release_msid");
    this.MusicService = AdditionalInfo.GetObject<string>(fields, "music_service");
    this.MusicServiceName = AdditionalInfo.GetObject<string>(fields, "music_service_name");
    this.OriginUrl = AdditionalInfo.GetObject<Uri>(fields, "origin_url");
    this.RecordingId = AdditionalInfo.GetValue<Guid>(fields, "recording_mbid");
    this.ReleaseArtistName = AdditionalInfo.GetObject<string>(fields, "release_artist_name");
    this.ReleaseArtistNames = AdditionalInfo.GetObjectList<string>(fields, "release_artist_names");
    this.ReleaseGroupId = AdditionalInfo.GetValue<Guid>(fields, "release_group_mbid");
    this.ReleaseId = AdditionalInfo.GetValue<Guid>(fields, "release_mbid");
    this.SpotifyAlbumId = AdditionalInfo.GetObject<Uri>(fields, "spotify_album_id");
    this.SpotifyAlbumArtistIds = AdditionalInfo.GetObjectList<Uri>(fields, "spotify_album_artist_ids");
    this.SpotifyArtistIds = AdditionalInfo.GetObjectList<Uri>(fields, "spotify_artist_ids");
    this.SpotifyId = AdditionalInfo.GetObject<Uri>(fields, "spotify_id");
    this.SubmissionClient = AdditionalInfo.GetObject<string>(fields, "submission_client");
    this.SubmissionClientVersion = AdditionalInfo.GetObject<string>(fields, "submission_client_version");
    this.Tags = AdditionalInfo.GetObjectList<string>(fields, "tags");
    this.TrackId = AdditionalInfo.GetValue<Guid>(fields, "track_mbid");
    this.TrackNumber = AdditionalInfo.GetValue<int>(fields, "tracknumber");
    this.WorkIds = AdditionalInfo.GetValueList<Guid>(fields, "work_mbids");
    // Extract well-known fields requiring a bit more work
    {
      var duration = AdditionalInfo.GetValue<int>(fields, "duration_ms");
      if (duration is not null) {
        this.Duration = TimeSpan.FromMilliseconds(duration.Value);
      }
    }
  }

  public IReadOnlyDictionary<string, object?> AllFields { get; }

  public IReadOnlyList<Guid?>? ArtistIds { get; }

  public IReadOnlyList<string?>? ArtistNames { get; }

  public int? DiscNumber { get; }

  public TimeSpan? Duration { get; }

  public Guid? ImportedArtistId { get; }

  public Guid? ImportedReleaseId { get; }

  public string? Isrc { get; }

  public string? ListeningFrom { get; }

  public string? MediaPlayer { get; }

  public string? MediaPlayerVersion { get; }

  public Guid? MessyArtistId { get; }

  public Guid? MessyRecordingId { get; }

  public Guid? MessyReleaseId { get; }

  public string? MusicService { get; }

  public string? MusicServiceName { get; }

  public Uri? OriginUrl { get; }

  public Guid? RecordingId { get; }

  public string? ReleaseArtistName { get; }

  public IReadOnlyList<string?>? ReleaseArtistNames { get; }

  public Guid? ReleaseGroupId { get; }

  public Guid? ReleaseId { get; }

  public IReadOnlyList<Uri?>? SpotifyAlbumArtistIds { get; }

  public Uri? SpotifyAlbumId { get; }

  public IReadOnlyList<Uri?>? SpotifyArtistIds { get; }

  public Uri? SpotifyId { get; }

  public string? SubmissionClient { get; }

  public string? SubmissionClientVersion { get; }

  public IReadOnlyList<string?>? Tags { get; }

  public Guid? TrackId { get; }

  public int? TrackNumber { get; }

  public IReadOnlyList<Guid?>? WorkIds { get; }

  #region Helper Methods

  private static T? GetObject<T>(Dictionary<string, object?> fields, string name) where T : class {
    if (fields.TryGetValue(name, out var value) && value is not null && value is T typedValue) {
      return typedValue;
    }
    return null;
  }

  private static IReadOnlyList<T?>? GetObjectList<T>(Dictionary<string, object?> fields, string name) where T : class {
    if (fields.TryGetValue(name, out var value) && value is not null) {
      if (value is IReadOnlyList<T?> list) {
        return list;
      }
      if (value is object[] { Length: 0 }) {
        return Array.Empty<T?>();
      }
    }
    return null;
  }

  private static T? GetValue<T>(Dictionary<string, object?> fields, string name) where T : struct {
    if (fields.TryGetValue(name, out var value) && value is not null && value is T typedValue) {
      return typedValue;
    }
    return null;
  }

  private static IReadOnlyList<T?>? GetValueList<T>(Dictionary<string, object?> fields, string name) where T : struct {
    if (fields.TryGetValue(name, out var value) && value is not null) {
      if (value is IReadOnlyList<T?> list) {
        return list;
      }
      if (value is IReadOnlyList<T> nonNullableList) { // convert to array of nullable
        var nullableList = new T?[nonNullableList.Count];
        for (var i = 0; i < nullableList.Length; ++i) {
          nullableList[i] = nonNullableList[i];
        }
        return nullableList;
      }
      if (value is object[] { Length: 0 }) {
        return Array.Empty<T?>();
      }
    }
    return null;
  }

  #endregion

}
