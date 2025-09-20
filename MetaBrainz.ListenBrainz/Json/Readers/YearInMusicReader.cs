using System;
using System.Collections.Generic;
using System.Text.Json;

using MetaBrainz.Common.Json;
using MetaBrainz.ListenBrainz.Interfaces;
using MetaBrainz.ListenBrainz.Objects;

namespace MetaBrainz.ListenBrainz.Json.Readers;

internal class YearInMusicReader : PayloadReader<YearInMusic> {

  public static readonly YearInMusicReader Instance = new();

  protected override YearInMusic ReadPayload(ref Utf8JsonReader reader, JsonSerializerOptions options) {
    IYearInMusicData? data = null;
    string? user = null;
    Dictionary<string, object?>? rest = null;
    while (reader.TokenType == JsonTokenType.PropertyName) {
      var prop = reader.GetPropertyName();
      try {
        reader.Read();
        switch (prop) {
          case "data":
            data = reader.GetObject(YearInMusicDataReader.Instance, options);
            break;
          case "user_name":
            user = reader.GetString();
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
    return new YearInMusic {
      Data = data ?? throw new MissingField("data"),
      User = user ?? throw new MissingField("user_name"),
      UnhandledProperties = rest,
    };
  }

}
