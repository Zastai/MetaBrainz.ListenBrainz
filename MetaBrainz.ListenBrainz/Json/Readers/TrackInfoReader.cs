using System;
using System.Collections.Generic;
using System.Text.Json;

using MetaBrainz.Common.Json;
using MetaBrainz.Common.Json.Converters;
using MetaBrainz.ListenBrainz.Interfaces;
using MetaBrainz.ListenBrainz.Objects;

namespace MetaBrainz.ListenBrainz.Json.Readers;

internal sealed class TrackInfoReader : ObjectReader<TrackInfo> {

  public static readonly TrackInfoReader Instance = new TrackInfoReader();

  protected override TrackInfo ReadObjectContents(ref Utf8JsonReader reader, JsonSerializerOptions options) {
    string? name = null;
    string? artist = null;
    string? release = null;
    IAdditionalInfo? info = null;
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
          case "release_name":
            release = reader.GetString();
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
    if (name == null) {
      throw new JsonException("Expected track name not found or null.");
    }
    if (artist == null) {
      throw new JsonException("Expected artist name not found or null.");
    }
    if (info == null) {
      throw new JsonException("Expected additional info not found or null.");
    }
    return new TrackInfo(name, artist, info) {
      Release = release,
      UnhandledProperties = rest
    };
  }

}
