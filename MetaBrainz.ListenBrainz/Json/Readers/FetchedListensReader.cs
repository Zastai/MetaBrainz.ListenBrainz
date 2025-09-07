using System;
using System.Collections.Generic;
using System.Text.Json;

using MetaBrainz.Common.Json;
using MetaBrainz.ListenBrainz.Interfaces;
using MetaBrainz.ListenBrainz.Objects;

namespace MetaBrainz.ListenBrainz.Json.Readers;

internal sealed class FetchedListensReader : PayloadReader<FetchedListens> {

  public static readonly FetchedListensReader Instance = new();

  protected override FetchedListens ReadPayload(ref Utf8JsonReader reader, JsonSerializerOptions options) {
    ushort? count = null;
    IReadOnlyList<IListen>? listens = null;
    long? newest = null;
    long? oldest = null;
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
            listens = reader.ReadList(ListenReader.Instance, options);
            break;
          case "latest_listen_ts":
            newest = reader.GetOptionalInt64();
            break;
          case "oldest_listen_ts":
            oldest = reader.GetOptionalInt64();
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
    if (newest is null) {
      throw new JsonException("Expected latest-listen timestamp not found or null.");
    }
    if (oldest is null) {
      throw new JsonException("Expected oldest-listen timestamp not found or null.");
    }
    return new FetchedListens {
      Listens = listens.VerifyPayloadContents(count),
      Newest = DateTimeOffset.FromUnixTimeSeconds(newest.Value),
      Oldest = DateTimeOffset.FromUnixTimeSeconds(oldest.Value),
      UnhandledProperties = rest,
      User = user ?? throw new JsonException("Expected user id not found or null."),
    };
  }

}
