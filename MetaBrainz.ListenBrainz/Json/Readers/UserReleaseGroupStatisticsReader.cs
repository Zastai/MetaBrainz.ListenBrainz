using System;
using System.Collections.Generic;
using System.Text.Json;

using MetaBrainz.Common.Json;
using MetaBrainz.ListenBrainz.Interfaces;
using MetaBrainz.ListenBrainz.Objects;

namespace MetaBrainz.ListenBrainz.Json.Readers;

internal sealed class UserReleaseGroupStatisticsReader : PayloadReader<UserReleaseGroupStatistics> {

  public static readonly UserReleaseGroupStatisticsReader Instance = new();

  protected override UserReleaseGroupStatistics ReadPayload(ref Utf8JsonReader reader, JsonSerializerOptions options) {
    IReadOnlyList<IReleaseGroupInfo>? releaseGroups = null;
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
          case "release_groups":
            releaseGroups = reader.ReadList(ReleaseGroupInfoReader.Instance, options);
            break;
          case "count":
            count = reader.GetInt32();
            break;
          case "from_ts": {
            var unixTime = reader.GetOptionalInt64();
            oldestListen = unixTime is null ? null : DateTimeOffset.FromUnixTimeSeconds(unixTime.Value);
            break;
          }
          case "last_updated":
            lastUpdated = DateTimeOffset.FromUnixTimeSeconds(reader.GetInt64());
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
          case "to_ts": {
            var unixTime = reader.GetOptionalInt64();
            newestListen = unixTime is null ? null : DateTimeOffset.FromUnixTimeSeconds(unixTime.Value);
            break;
          }
          case "total_release_group_count":
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
    releaseGroups = PayloadReader<UserReleaseGroupStatistics>.VerifyPayloadContents(count, releaseGroups);
    if (lastUpdated is null) {
      throw new JsonException("Expected last-updated timestamp not found or null.");
    }
    if (range is null) {
      throw new JsonException("Expected range not found or null.");
    }
    if (user is null) {
      throw new JsonException("Expected user id not found or null.");
    }
    return new UserReleaseGroupStatistics(lastUpdated.Value, range.Value, user) {
      NewestListen = newestListen,
      Offset = offset,
      OldestListen = oldestListen,
      ReleaseGroups = releaseGroups,
      TotalCount = totalCount,
      UnhandledProperties = rest,
    };
  }

}
