using System;
using System.Collections.Generic;
using System.Text.Json;

using MetaBrainz.Common.Json;
using MetaBrainz.Common.Json.Converters;
using MetaBrainz.ListenBrainz.Objects;

namespace MetaBrainz.ListenBrainz.Json.Readers;

internal sealed class GenreActivityDetailsReader : ObjectReader<GenreActivityDetails> {

  public static readonly GenreActivityDetailsReader Instance = new();

  protected override GenreActivityDetails ReadObjectContents(ref Utf8JsonReader reader, JsonSerializerOptions options) {
    string? genre = null;
    int? hour = null;
    int? listenCount = null;
    Dictionary<string, object?>? rest = null;
    while (reader.TokenType == JsonTokenType.PropertyName) {
      var prop = reader.GetPropertyName();
      try {
        reader.Read();
        switch (prop) {
          case "genre":
            genre = reader.GetString();
            break;
          case "hour":
            hour = reader.GetInt32();
            break;
          case "listen_count":
            listenCount = reader.GetInt32();
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
    if (hour is null) {
      throw new JsonException("Expected hour not found or null.");
    }
    if (hour is < 0 or > 23) {
      throw new JsonException($"The specified hour ({hour}) is out of range (should be 0-23).");
    }
    return new GenreActivityDetails {
      Genre = genre ?? throw new JsonException("Expected genre not found or null."),
      Hour = hour.Value,
      ListenCount = listenCount ?? throw new JsonException("Expected listen count not found or null."),
      UnhandledProperties = rest,
    };
  }

}
