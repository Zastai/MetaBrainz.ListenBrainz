using System;
using System.Collections.Generic;
using System.Text.Json;

using MetaBrainz.Common.Json;
using MetaBrainz.ListenBrainz.Interfaces;
using MetaBrainz.ListenBrainz.Objects;

namespace MetaBrainz.ListenBrainz.Json.Readers;

internal sealed class SiteArtistStatisticsReader : PayloadReader<SiteArtistStatistics> {

  public static readonly SiteArtistStatisticsReader Instance = new SiteArtistStatisticsReader();

  protected override SiteArtistStatistics ReadPayload(ref Utf8JsonReader reader, JsonSerializerOptions options) {
    IReadOnlyList<IArtistTimeRange>? timeRanges = null;
    int? count = null;
    DateTimeOffset? lastUpdated = null;
    DateTimeOffset? newestListen = null;
    int? offset = null;
    DateTimeOffset? oldestListen = null;
    StatisticsRange? range = null;
    Dictionary<string, object?>? rest = null;
    while (reader.TokenType == JsonTokenType.PropertyName) {
      var prop = reader.GetPropertyName();
      try {
        reader.Read();
        switch (prop) {
          case "count":
            count = reader.GetInt32();
            break;
          case "from_ts":
            oldestListen = UnixTime.Convert(reader.GetOptionalInt64());
            break;
          case "last_updated":
            lastUpdated = UnixTime.Convert(reader.GetInt64());
            break;
          case "offset":
            offset = reader.GetInt32();
            break;
          case "range":
            range = EnumHelper.ParseStatisticsRange(reader.GetString());
            if (range == StatisticsRange.Unknown) {
              goto default; // also register it as an unhandled property
            }
            break;
          case "time_ranges":
            timeRanges = reader.ReadList(ArtistTimeRangeReader.Instance, options);
            break;
          case "to_ts":
            newestListen = UnixTime.Convert(reader.GetOptionalInt64());
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
    if (count == null) {
      throw new JsonException("Expected count not found or null.");
    }
    if (lastUpdated == null) {
      throw new JsonException("Expected last-updated timestamp not found or null.");
    }
    if (offset == null) {
      throw new JsonException("Expected offset not found or null.");
    }
    if (range == null) {
      throw new JsonException("Expected range not found or null.");
    }
    return new SiteArtistStatistics(count.Value, lastUpdated.Value, offset.Value, range.Value) {
      NewestListen = newestListen,
      OldestListen = oldestListen,
      TimeRanges = timeRanges,
      UnhandledProperties = rest,
    };
  }

}
