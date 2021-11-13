using System;
using System.Collections.Generic;
using System.Text.Json;

using MetaBrainz.Common.Json;
using MetaBrainz.ListenBrainz.Interfaces;
using MetaBrainz.ListenBrainz.Objects;

namespace MetaBrainz.ListenBrainz.Json.Readers;

internal sealed class UserDailyActivityReader : PayloadReader<UserDailyActivity> {

  public static readonly UserDailyActivityReader Instance = new UserDailyActivityReader();

  protected override UserDailyActivity ReadPayload(ref Utf8JsonReader reader, JsonSerializerOptions options) {
    IDailyActivity? activity = null;
    DateTimeOffset? lastUpdated = null;
    DateTimeOffset? newestListen = null;
    DateTimeOffset? oldestListen = null;
    StatisticsRange? range = null;
    string? user = null;
    Dictionary<string, object?>? rest = null;
    while (reader.TokenType == JsonTokenType.PropertyName) {
      var prop = reader.GetPropertyName();
      try {
        reader.Read();
        switch (prop) {
          case "daily_activity":
            activity = reader.GetOptionalObject(DailyActivityReader.Instance, options);
            break;
          case "from_ts":
            oldestListen = UnixTime.Convert(reader.GetOptionalInt64());
            break;
          case "last_updated":
            lastUpdated = UnixTime.Convert(reader.GetInt64());
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
    if (lastUpdated == null) {
      throw new JsonException("Expected last-updated timestamp not found or null.");
    }
    if (range == null) {
      throw new JsonException("Expected range not found or null.");
    }
    if (user == null) {
      throw new JsonException("Expected user id not found or null.");
    }
    return new UserDailyActivity(lastUpdated.Value, range.Value, user) {
      Activity = activity,
      NewestListen = newestListen,
      OldestListen = oldestListen,
      UnhandledProperties = rest,
    };
  }

}
