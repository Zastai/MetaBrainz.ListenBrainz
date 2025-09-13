using System;
using System.Collections.Generic;
using System.Text.Json;

using MetaBrainz.Common.Json;
using MetaBrainz.ListenBrainz.Interfaces;
using MetaBrainz.ListenBrainz.Objects;

namespace MetaBrainz.ListenBrainz.Json.Readers;

internal sealed class ArtistListenersReader : PayloadReader<ArtistListeners> {

  public static readonly ArtistListenersReader Instance = new();

  protected override ArtistListeners ReadPayload(ref Utf8JsonReader reader, JsonSerializerOptions options) {
    Guid? mbid = null;
    DateTimeOffset? lastUpdated = null;
    DateTimeOffset? newestListen = null;
    DateTimeOffset? oldestListen = null;
    string? name = null;
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
          case "artist_mbid":
            mbid = reader.GetOptionalGuid();
            break;
          case "artist_name":
            name = reader.GetString();
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
    return new ArtistListeners {
      Id = mbid ?? throw new JsonException("Expected artist MBID not found or null."),
      LastUpdated = lastUpdated ?? throw new JsonException("Expected last-updated timestamp not found or null."),
      Name = name ?? throw new JsonException("Expected artist name not found or null."),
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
