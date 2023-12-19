using System;
using System.Collections.Generic;
using System.Text.Json;

using MetaBrainz.Common.Json;
using MetaBrainz.ListenBrainz.Interfaces;
using MetaBrainz.ListenBrainz.Objects;

namespace MetaBrainz.ListenBrainz.Json.Readers;

internal sealed class PlayingNowReader : PayloadReader<PlayingNow> {

  public static readonly PlayingNowReader Instance = new();

  protected override PlayingNow ReadPayload(ref Utf8JsonReader reader, JsonSerializerOptions options) {
    int? count = null;
    IPlayingTrack? track = null;
    bool? playingNow = null;
    string? user = null;
    Dictionary<string, object?>? rest = null;
    while (reader.TokenType == JsonTokenType.PropertyName) {
      var prop = reader.GetPropertyName();
      try {
        reader.Read();
        switch (prop) {
          case "count":
            count = reader.GetUInt16();
            break;
          case "listens":
            if (reader.TokenType == JsonTokenType.Null) {
              track = null;
            }
            else if (reader.TokenType == JsonTokenType.StartArray) {
              reader.Read();
              if (reader.TokenType != JsonTokenType.EndArray) {
                track = reader.GetObject(PlayingTrackReader.Instance, options);
                reader.Read();
              }
              if (reader.TokenType != JsonTokenType.EndArray) {
                throw new JsonException("Expected end of listen list not found (too many listens returned?).");
              }
            }
            else {
              throw new JsonException("Invalid contents; should be an array or null.");
            }
            break;
          case "playing_now":
            playingNow = reader.GetBoolean();
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
    if (count is null) {
      throw new JsonException("Expected listen count not found or null.");
    }
    if (count > 1) {
      throw new JsonException($"Too many listens reported (expected at most one; got {count}).");
    }
    if (count == 1 && track is null) {
      throw new JsonException("No listen data found, but the listen count is 1.");
    }
    if (count == 0 && track is not null) {
      throw new JsonException("Listen data found, but the listen count is 0.");
    }
    if (user is null) {
      throw new JsonException("Expected user id not found or null.");
    }
    if (playingNow is not true) {
      throw new JsonException("Expected 'playing now' flag not found or set incorrectly.");
    }
    return new PlayingNow(track, user) {
      UnhandledProperties = rest
    };
  }

}
