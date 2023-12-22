using System;
using System.Collections.Generic;
using System.Text.Json;

using MetaBrainz.Common.Json;
using MetaBrainz.Common.Json.Converters;
using MetaBrainz.ListenBrainz.Interfaces;
using MetaBrainz.ListenBrainz.Objects;

namespace MetaBrainz.ListenBrainz.Json.Readers;

internal sealed class MusicBrainzIdMappingsReader : ObjectReader<MusicBrainzIdMappings> {

  public static readonly MusicBrainzIdMappingsReader Instance = new();

  protected override MusicBrainzIdMappings ReadObjectContents(ref Utf8JsonReader reader, JsonSerializerOptions options) {
    IReadOnlyList<Guid>? artists = null;
    long? caaId = null;
    Guid? caaRelease = null;
    IReadOnlyList<IArtistCredit>? credits = null;
    string? name = null;
    Guid? recording = null;
    Guid? release = null;
    Dictionary<string, object?>? rest = null;
    while (reader.TokenType == JsonTokenType.PropertyName) {
      var prop = reader.GetPropertyName();
      try {
        reader.Read();
        switch (prop) {
          case "artist_mbids":
            artists = reader.ReadList<Guid>(options);
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
          case "recording_mbid":
            recording = reader.GetOptionalGuid();
            break;
          case "recording_name":
            name = reader.GetString();
            break;
          case "release_mbid":
            release= reader.GetOptionalGuid();
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
    return new MusicBrainzIdMappings {
      ArtistIds = artists,
      Credits = credits,
      CoverArtId = caaId,
      CoverArtReleaseId = caaRelease,
      RecordingId = recording,
      RecordingName = name,
      ReleaseId = release,
      UnhandledProperties = rest
    };
  }

}
