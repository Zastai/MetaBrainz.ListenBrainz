using System;
using System.Collections.Generic;
using System.Text.Json;

using MetaBrainz.Common.Json;
using MetaBrainz.Common.Json.Converters;
using MetaBrainz.ListenBrainz.Objects;

namespace MetaBrainz.ListenBrainz.Json.Readers;

internal sealed class ListenTimeRangeReader : ObjectReader<ListenTimeRange> {

  public static readonly ListenTimeRangeReader Instance = new();

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
          case "from_ts": {
            var unixTime = reader.GetOptionalInt64();
            rangeStart = unixTime is null ? null : DateTimeOffset.FromUnixTimeSeconds(unixTime.Value);
            break;
          }
          case "listen_count":
            listenCount = reader.GetInt32();
            break;
          case "time_range":
            description = reader.GetString();
            break;
          case "to_ts": {
            var unixTime = reader.GetOptionalInt64();
            rangeEnd = unixTime is null ? null : DateTimeOffset.FromUnixTimeSeconds(unixTime.Value);
            break;
          }
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
    return new ListenTimeRange {
      Description = description ?? throw new JsonException("Expected description not found or null."),
      ListenCount = listenCount ?? throw new JsonException("Expected listen count not found or null."),
      RangeEnd = rangeEnd,
      RangeStart = rangeStart,
      UnhandledProperties = rest,
    };
  }

}
