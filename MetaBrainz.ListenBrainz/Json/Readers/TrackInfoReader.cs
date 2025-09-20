using System;
using System.Collections.Generic;
using System.Text.Json;

using MetaBrainz.Common.Json;
using MetaBrainz.Common.Json.Converters;
using MetaBrainz.ListenBrainz.Interfaces;
using MetaBrainz.ListenBrainz.Objects;

namespace MetaBrainz.ListenBrainz.Json.Readers;

internal sealed class TrackInfoReader : ObjectReader<TrackInfo> {

  public static readonly TrackInfoReader Instance = new();

  protected override TrackInfo ReadObjectContents(ref Utf8JsonReader reader, JsonSerializerOptions options) {
    string? name = null;
    string? artist = null;
    string? release = null;
    IAdditionalInfo? info = null;
    IMusicBrainzIdMappings? mapping = null;
    Dictionary<string, object?>? rest = null;
    while (reader.TokenType == JsonTokenType.PropertyName) {
      var prop = reader.GetPropertyName();
      try {
        reader.Read();
        switch (prop) {
          case "additional_info":
            info = reader.GetObject(AdditionalInfoReader.Instance, options);
            break;
          case "artist_name":
            artist = reader.GetString();
            break;
          case "mbid_mapping":
            mapping = reader.GetObject(MusicBrainzIdMappingsReader.Instance, options);
            break;
          case "release_name":
            release = reader.GetString();
            break;
          case "track_name":
            name = reader.GetString();
            break;
          default:
            rest ??= [ ];
            rest[prop] = reader.GetOptionalObject(options);
            break;
        }
      }
      catch (Exception e) {
        throw new JsonException($"Failed to deserialize the '{prop}' property.", e);
      }
      reader.Read();
    }
    return new TrackInfo {
      AdditionalInfo = info ?? throw new JsonException("Expected additional info not found or null."),
      Artist = artist ?? throw new JsonException("Expected artist name not found or null."),
      Name = name ?? throw new JsonException("Expected track name not found or null."),
      MusicBrainzIdMappings = mapping,
      Release = release,
      UnhandledProperties = rest,
    };
  }

}
