using System;
using System.Collections.Generic;
using System.Text.Json;

using MetaBrainz.Common.Json;
using MetaBrainz.ListenBrainz.Interfaces;
using MetaBrainz.ListenBrainz.Objects;

namespace MetaBrainz.ListenBrainz.Json.Readers;

internal sealed class ReleaseGroupListenersReader : PayloadReader<ReleaseGroupListeners> {

  public static readonly ReleaseGroupListenersReader Instance = new();

  protected override ReleaseGroupListeners ReadPayload(ref Utf8JsonReader reader, JsonSerializerOptions options) {
    IReadOnlyList<Guid>? artistMbids = null;
    string? artistName = null;
    long? caaId = null;
    Guid? caaRelease = null;
    Guid? mbid = null;
    string? name = null;
    DateTimeOffset? lastUpdated = null;
    DateTimeOffset? newestListen = null;
    DateTimeOffset? oldestListen = null;
    StatisticsRange? range = null;
    Dictionary<string, object?>? rest = null;
    IReadOnlyList<ITopListener>? topListeners = null;
    int? totalListeners = null;
    int? totalListens = null;
    while (reader.TokenType == JsonTokenType.PropertyName) {
      var prop = reader.GetPropertyName();
      try {
        reader.Read();
        switch (prop) {
          case "artist_mbids":
            artistMbids = reader.ReadList<Guid>(options);
            break;
          case "artist_name":
            artistName = reader.GetString();
            break;
          case "caa_id":
            caaId = reader.GetOptionalInt64();
            break;
          case "caa_release_mbid":
            caaRelease = reader.GetOptionalGuid();
            break;
          case "from_ts": {
            var unixTime = reader.GetOptionalInt64();
            oldestListen = unixTime is null ? null : DateTimeOffset.FromUnixTimeSeconds(unixTime.Value);
            break;
          }
          case "last_updated":
            lastUpdated = DateTimeOffset.FromUnixTimeSeconds(reader.GetInt64());
            break;
          case "listeners":
            topListeners = reader.ReadList(TopListenerReader.Instance, options);
            break;
          case "release_group_mbid":
            mbid = reader.GetOptionalGuid();
            break;
          case "release_group_name":
            name = reader.GetString();
            break;
          case "range":
          case "stats_range":
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
          case "total_listen_count":
            totalListens = reader.GetInt32();
            break;
          case "total_user_count":
            totalListeners = reader.GetInt32();
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
    return new ReleaseGroupListeners {
      ArtistIds = artistMbids,
      ArtistName = artistName,
      CoverArtId = caaId,
      CoverArtReleaseGroupId = caaRelease,
      Id = mbid ?? throw new JsonException("Expected release group MBID not found or null."),
      LastUpdated = lastUpdated ?? throw new JsonException("Expected last-updated timestamp not found or null."),
      Name = name ?? throw new JsonException("Expected release group name not found or null."),
      NewestListen = newestListen,
      OldestListen = oldestListen,
      Range = range ?? throw new JsonException("Expected range not found or null."),
      TopListeners = topListeners ?? throw new JsonException("Expected list of top listeners not found or null."),
      TotalListeners = totalListeners ?? throw new JsonException("Expected total listener count not found or null."),
      TotalListens = totalListens ?? throw new JsonException("Expected total listener count not found or null."),
      UnhandledProperties = rest,
    };
  }

}
