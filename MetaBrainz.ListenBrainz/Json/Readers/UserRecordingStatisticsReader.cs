using System;
using System.Collections.Generic;
using System.Text.Json;

using MetaBrainz.Common.Json;
using MetaBrainz.ListenBrainz.Interfaces;
using MetaBrainz.ListenBrainz.Objects;

namespace MetaBrainz.ListenBrainz.Json.Readers;

internal sealed class UserRecordingStatisticsReader : PayloadReader<UserRecordingStatistics> {

  public static readonly UserRecordingStatisticsReader Instance = new();

  protected override UserRecordingStatistics ReadPayload(ref Utf8JsonReader reader, JsonSerializerOptions options) {
    IReadOnlyList<IRecordingInfo>? recordings = null;
    int? count = null;
    DateTimeOffset? lastUpdated = null;
    DateTimeOffset? newestListen = null;
    int? offset = null;
    DateTimeOffset? oldestListen = null;
    StatisticsRange? range = null;
    int? totalCount = null;
    string? user = null;
    Dictionary<string, object?>? rest = null;
    while (reader.TokenType == JsonTokenType.PropertyName) {
      var prop = reader.GetPropertyName();
      try {
        reader.Read();
        switch (prop) {
          case "recordings":
            recordings = reader.ReadList(RecordingInfoReader.Instance, options);
            break;
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
          case "to_ts":
            newestListen = UnixTime.Convert(reader.GetOptionalInt64());
            break;
          case "total_recording_count":
            totalCount = reader.GetInt32();
            break;
          case "user_id":
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
    recordings = PayloadReader<UserRecordingStatistics>.VerifyPayloadContents(count, recordings);
    if (lastUpdated == null) {
      throw new JsonException("Expected last-updated timestamp not found or null.");
    }
    if (range == null) {
      throw new JsonException("Expected range not found or null.");
    }
    if (user == null) {
      throw new JsonException("Expected user id not found or null.");
    }
    return new UserRecordingStatistics(lastUpdated.Value, range.Value, user) {
      NewestListen = newestListen,
      Offset = offset,
      OldestListen = oldestListen,
      Recordings = recordings,
      TotalCount = totalCount,
      UnhandledProperties = rest,
    };
  }

}
