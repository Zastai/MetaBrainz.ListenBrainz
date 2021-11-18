using System;
using System.Collections.Generic;
using System.Text.Json;

using MetaBrainz.Common.Json;
using MetaBrainz.Common.Json.Converters;
using MetaBrainz.ListenBrainz.Objects;

namespace MetaBrainz.ListenBrainz.Json.Readers;

internal sealed class MusicBrainzIdMappingsReader : ObjectReader<MusicBrainzIdMappings> {
  
  public static readonly MusicBrainzIdMappingsReader Instance = new();

  protected override MusicBrainzIdMappings ReadObjectContents(ref Utf8JsonReader reader, JsonSerializerOptions options) {
    IReadOnlyList<Guid>? artists = null;
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
          case "recording_mbid":
            recording = reader.GetOptionalGuid();
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
      RecordingId = recording,
      ReleaseId = release,
      UnhandledProperties = rest
    };
  }

}
