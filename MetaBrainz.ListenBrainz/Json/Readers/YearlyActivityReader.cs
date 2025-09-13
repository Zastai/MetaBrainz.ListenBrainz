using System;
using System.Collections.Generic;
using System.Text.Json;

using MetaBrainz.Common.Json;
using MetaBrainz.Common.Json.Converters;
using MetaBrainz.ListenBrainz.Objects;

namespace MetaBrainz.ListenBrainz.Json.Readers;

internal sealed class YearlyActivityReader : ObjectReader<YearlyActivity> {

  public static readonly YearlyActivityReader Instance = new();

  protected override YearlyActivity ReadObjectContents(ref Utf8JsonReader reader, JsonSerializerOptions options) {
    int? listenCount = null;
    int? year = null;
    Dictionary<string, object?>? rest = null;
    while (reader.TokenType == JsonTokenType.PropertyName) {
      var prop = reader.GetPropertyName();
      try {
        reader.Read();
        switch (prop) {
          case "listen_count":
            listenCount = reader.GetInt32();
            break;
          case "year":
            year = reader.GetInt32();
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
    return new YearlyActivity {
      ListenCount = listenCount ?? throw new JsonException("Expected listen count not found or null."),
      UnhandledProperties = rest,
      Year = year ?? throw new JsonException("Expected year not found or null."),
    };
  }

}
