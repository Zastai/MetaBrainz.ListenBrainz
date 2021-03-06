using System;
using System.Collections.Generic;
using System.Text.Json;

using MetaBrainz.Common.Json;
using MetaBrainz.Common.Json.Converters;
using MetaBrainz.ListenBrainz.Objects;

namespace MetaBrainz.ListenBrainz.Json.Readers {

  internal sealed class HourlyActivityReader : ObjectReader<HourlyActivity> {

    public static readonly HourlyActivityReader Instance = new HourlyActivityReader();

    protected override HourlyActivity ReadObjectContents(ref Utf8JsonReader reader, JsonSerializerOptions options) {
      int? hour = null;
      int? listenCount = null;
      Dictionary<string, object?>? rest = null;
      while (reader.TokenType == JsonTokenType.PropertyName) {
        var prop = reader.GetPropertyName();
        try {
          reader.Read();
          switch (prop) {
            case "hour":
              hour = reader.GetInt32();
              break;
            case "listen_count":
              listenCount = reader.GetInt32();
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
      if (!hour.HasValue)
        throw new JsonException("Expected hour not found or null.");
      if (hour < 0 || hour > 23)
        throw new JsonException($"The specified hour ({hour}) is out of range (should be 0-23).");
      if (!listenCount.HasValue)
        throw new JsonException("Expected listen count not found or null.");
      return new HourlyActivity(hour.Value, listenCount.Value) {
        UnhandledProperties = rest
      };
    }

  }

}
