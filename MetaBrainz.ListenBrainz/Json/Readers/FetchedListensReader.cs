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
    string? user = null;
    long? ts = null;
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
            ts = reader.GetInt64();
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
    listens = this.VerifyPayloadContents(count, listens);
    if (user == null) {
      throw new JsonException("Expected user id not found or null.");
    }
    if (!ts.HasValue) {
      throw new JsonException("Expected latest-listen timestamp not found or null.");
    }
    return new FetchedListens(listens, ts.Value, user) {
      UnhandledProperties = rest
    };
  }

}
