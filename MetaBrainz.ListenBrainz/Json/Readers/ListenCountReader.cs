using System;
using System.Collections.Generic;
using System.Text.Json;

using MetaBrainz.Common.Json;
using MetaBrainz.ListenBrainz.Objects;

namespace MetaBrainz.ListenBrainz.Json.Readers;

internal sealed class ListenCountReader : PayloadReader<ListenCount> {

  public static readonly ListenCountReader Instance = new ListenCountReader();

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
    if (!count.HasValue) {
      throw new JsonException("Expected listen count not found or null.");
    }
    return new ListenCount(count.Value) {
      UnhandledProperties = rest
    };
  }

}
