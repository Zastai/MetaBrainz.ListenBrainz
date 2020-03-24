using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

using JetBrains.Annotations;

using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects {

  [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
  internal sealed class AdditionalInfo : IAdditionalInfo {

    IReadOnlyDictionary<string, object> IAdditionalInfo.AllFields => this.AllFields;

    [JsonExtensionData]
    public Dictionary<string, object> AllFields { get; set; }

    public IReadOnlyList<Guid> ArtistIds => this.GetListField<Guid>("artist_mbids");

    public IReadOnlyList<string> ArtistNames => this.GetListField<string>("artist_names");

    public int? DiscNumber => (int?) this.GetValueTypedField<long>("discnumber");

    public TimeSpan? Duration {
      get {
        var duration = this.GetValueTypedField<long>("duration_ms");
        if (duration.HasValue)
          return TimeSpan.FromMilliseconds(duration.Value);
        return null;
      }
    }

    public string Isrc => this.GetTypedField<string>("isrc");

    public string ListeningFrom => this.GetTypedField<string>("listening_from");

    public Guid? MessyArtistId => this.GetValueTypedField<Guid>("artist_msid");

    public Guid? MessyRecordingId => this.GetValueTypedField<Guid>("recording_msid");

    public Guid? MessyReleaseId => this.GetValueTypedField<Guid>("release_msid");

    public Guid? RecordingId => this.GetValueTypedField<Guid>("recording_mbid");

    public string ReleaseArtistName => this.GetTypedField<string>("release_artist_name");

    public IReadOnlyList<string> ReleaseArtistNames => this.GetListField<string>("release_artist_names");

    public Guid? ReleaseGroupId => this.GetValueTypedField<Guid>("release_group_mbid");

    public Guid? ReleaseId => this.GetValueTypedField<Guid>("release_mbid");

    public IReadOnlyList<Uri> SpotifyAlbumArtistIds => this.GetListField<Uri>("spotify_album_artist_ids");

    public Uri SpotifyAlbumId => this.GetTypedField<Uri>("spotify_album_id");

    public IReadOnlyList<Uri> SpotifyArtistIds => this.GetListField<Uri>("spotify_artist_ids");

    public Uri SpotifyId => this.GetTypedField<Uri>("spotify_id");

    public IReadOnlyList<string> Tags => this.GetListField<string>("tags");

    public Guid? TrackId => this.GetValueTypedField<Guid>("track_mbid");

    public int? TrackNumber => (int?) this.GetValueTypedField<long>("tracknumber");

    public IReadOnlyList<Guid> WorkIds => this.GetListField<Guid>("work_mbids");

    private IReadOnlyList<T> GetListField<T>(string name) {
      if (this.AllFields != null && this.AllFields.TryGetValue(name, out var value) && value is T[] array)
        return array;
      return null;
    }

    private T GetTypedField<T>(string name) where T : class {
      if (this.AllFields != null && this.AllFields.TryGetValue(name, out var value) && value is T typedValue)
        return typedValue;
      return null;
    }

    private T? GetValueTypedField<T>(string name) where T : struct {
      if (this.AllFields != null && this.AllFields.TryGetValue(name, out var value) && value is T typedValue)
        return typedValue;
      return null;
    }

  }

}
