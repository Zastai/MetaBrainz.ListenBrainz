using System;
using System.Collections.Generic;
using System.Text.Json;

using MetaBrainz.Common.Json;
using MetaBrainz.Common.Json.Converters;
using MetaBrainz.ListenBrainz.Objects;

namespace MetaBrainz.ListenBrainz.Json.Readers;

internal class RecordingInfoReader : ObjectReader<RecordingInfo> {

  public static readonly RecordingInfoReader Instance = new();

  protected override RecordingInfo ReadObjectContents(ref Utf8JsonReader reader, JsonSerializerOptions options) {
    IReadOnlyList<Guid>? artistMbids = null;
    Guid? artistMsid = null;
    string? artistName = null;
    int? listenCount = null;
    Guid? mbid = null;
    Guid? msid = null;
    string? name = null;
    Guid? releaseMbid = null;
    Guid? releaseMsid = null;
    string? releaseName = null;
    Dictionary<string, object?>? rest = null;
    while (reader.TokenType == JsonTokenType.PropertyName) {
      var prop = reader.GetPropertyName();
      try {
        reader.Read();
        switch (prop) {
          case "artist_mbids":
            artistMbids = reader.ReadList<Guid>(options);
            break;
          case "artist_msid":
            artistMsid = reader.GetOptionalGuid();
            break;
          case "artist_name":
            artistName = reader.GetString();
            break;
          case "listen_count":
            listenCount = reader.GetInt32();
            break;
          case "recording_mbid":
            mbid = reader.GetOptionalGuid();
            break;
          case "recording_msid":
            msid = reader.GetOptionalGuid();
            break;
          case "release_mbid":
            releaseMbid = reader.GetOptionalGuid();
            break;
          case "release_msid":
            releaseMsid = reader.GetOptionalGuid();
            break;
          case "release_name":
            releaseName = reader.GetString();
            break;
          case "track_name":
            name = reader.GetString();
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
    if (listenCount == null) {
      throw new JsonException("Expected listen count not found or null.");
    }
    if (name == null) {
      throw new JsonException("Expected recording name not found or null.");
    }
    return new RecordingInfo(name, listenCount.Value) {
      ArtistIds = artistMbids,
      ArtistMessyId = artistMsid,
      ArtistName = artistName,
      Id = mbid,
      MessyId = msid,
      ReleaseId = releaseMbid,
      ReleaseMessyId = releaseMsid,
      ReleaseName = releaseName,
      UnhandledProperties = rest,
    };
  }

}
