using System;
using System.Collections.Generic;
using System.Text.Json;

using MetaBrainz.Common.Json;
using MetaBrainz.Common.Json.Converters;
using MetaBrainz.ListenBrainz.Interfaces;
using MetaBrainz.ListenBrainz.Objects;

namespace MetaBrainz.ListenBrainz.Json.Readers;

internal class TopRecordingReader : ObjectReader<TopRecording> {

  public static readonly TopRecordingReader Instance = new();

  protected override TopRecording ReadObjectContents(ref Utf8JsonReader reader, JsonSerializerOptions options) {
    IReadOnlyList<Guid>? artistMbids = null;
    string? artistName = null;
    long? caaId = null;
    Guid? caaRelease = null;
    IReadOnlyList<IArtistCredit>? credits = null;
    int? listenCount = null;
    Guid? mbid = null;
    string? name = null;
    Guid? releaseId = null;
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
          case "artist_name":
            artistName = reader.GetString();
            break;
          case "artists":
            credits = reader.ReadList(ArtistCreditReader.Instance, options);
            break;
          case "caa_id":
            caaId = reader.GetOptionalInt64();
            break;
          case "caa_release_mbid":
            caaRelease = reader.GetOptionalGuid();
            break;
          case "listen_count":
            listenCount = reader.GetInt32();
            break;
          case "recording_mbid":
            mbid = reader.GetOptionalGuid();
            break;
          case "release_mbid":
            releaseId = reader.GetOptionalGuid();
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
    return new TopRecording {
      ArtistIds = artistMbids,
      ArtistName = artistName,
      Credits = credits,
      CoverArtId = caaId,
      CoverArtReleaseGroupId = caaRelease,
      Id = mbid,
      ListenCount = listenCount ?? throw new JsonException("Expected listen count not found or null."),
      Name = name ?? throw new JsonException("Expected release group name not found or null."),
      ReleaseId = releaseId,
      ReleaseName = releaseName,
      UnhandledProperties = rest,
    };
  }

}
