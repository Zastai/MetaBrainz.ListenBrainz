using System;
using System.Collections.Generic;
using System.Text.Json;

using MetaBrainz.Common.Json;
using MetaBrainz.ListenBrainz.Objects;

namespace MetaBrainz.ListenBrainz.Json.Readers;

internal sealed class ListenCountReader : PayloadReader<ListenCount> {

  public static readonly ListenCountReader Instance = new();

  protected override ListenCount ReadPayload(ref Utf8JsonReader reader, JsonSerializerOptions options) {
    long? count = null;
    Dictionary<string, object?>? rest = null;
    while (reader.TokenType == JsonTokenType.PropertyName) {
      var prop = reader.GetPropertyName();
      try {
        reader.Read();
        switch (prop) {
          case "count":
            count = reader.GetInt64();
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
    return new ListenCount {
      Count = count ?? throw new JsonException("Expected listen count not found or null."),
      UnhandledProperties = rest,
    };
  }

}
