using System;
using System.Collections.Generic;
using System.Text.Json;

using MetaBrainz.Common.Json;
using MetaBrainz.Common.Json.Converters;
using MetaBrainz.ListenBrainz.Objects;

namespace MetaBrainz.ListenBrainz.Json.Readers;

internal sealed class TopGenreReader : ObjectReader<TopGenre> {

  public static readonly TopGenreReader Instance = new();

  protected override TopGenre ReadObjectContents(ref Utf8JsonReader reader, JsonSerializerOptions options) {
    int? count = null;
    string? genre = null;
    decimal? percentage = null;
    Dictionary<string, object?>? rest = null;
    while (reader.TokenType == JsonTokenType.PropertyName) {
      var prop = reader.GetPropertyName();
      try {
        reader.Read();
        switch (prop) {
          case "genre":
            genre = reader.GetString();
            break;
          case "genre_count":
            count = reader.GetInt32();
            break;
          case "genre_count_percent":
            percentage = reader.GetDecimal();
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
    return new TopGenre {
      Genre = genre ?? throw new MissingField("genre"),
      ListenCount = count ?? throw new MissingField("genre_count"),
      Percentage = percentage ?? throw new MissingField("genre_count_percent"),
      UnhandledProperties = rest,
    };
  }

}
