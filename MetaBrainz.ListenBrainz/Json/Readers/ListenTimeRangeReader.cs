using System;
using System.Collections.Generic;
using System.Text.Json;

using MetaBrainz.Common.Json;
using MetaBrainz.Common.Json.Converters;
using MetaBrainz.ListenBrainz.Objects;

namespace MetaBrainz.ListenBrainz.Json.Readers {

  internal sealed class ListenTimeRangeReader : ObjectReader<ListenTimeRange> {

    public static readonly ListenTimeRangeReader Instance = new ListenTimeRangeReader();

    protected override ListenTimeRange ReadObjectContents(ref Utf8JsonReader reader, JsonSerializerOptions options) {
      string? description = null;
      int? listenCount = null;
      DateTimeOffset? rangeEnd = null;
      DateTimeOffset? rangeStart = null;
      Dictionary<string, object?>? rest = null;
      while (reader.TokenType == JsonTokenType.PropertyName) {
        var prop = reader.GetPropertyName();
        try {
          reader.Read();
          switch (prop) {
            case "from_ts":
              rangeStart = UnixTime.Convert(reader.GetOptionalInt64());
              break;
            case "listen_count":
              listenCount = reader.GetInt32();
              break;
            case "time_range":
              description = reader.GetString();
              break;
            case "to_ts":
              rangeEnd = UnixTime.Convert(reader.GetOptionalInt64());
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
      if (description == null)
        throw new JsonException("Expected description not found or null.");
      if (listenCount == null)
        throw new JsonException("Expected listen count not found or null.");
      return new ListenTimeRange(description, listenCount.Value) {
        RangeEnd = rangeEnd,
        RangeStart = rangeStart,
        UnhandledProperties = rest,
      };
    }

  }

}
