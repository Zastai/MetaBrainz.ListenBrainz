using System;
using System.Collections.Generic;
using System.Text.Json;

using MetaBrainz.Common.Json;
using MetaBrainz.Common.Json.Converters;
using MetaBrainz.ListenBrainz.Interfaces;
using MetaBrainz.ListenBrainz.Objects;

namespace MetaBrainz.ListenBrainz.Json.Readers {

  internal sealed class FetchedListensReader : ListenPayloadReader<FetchedListens> {

    public static readonly FetchedListensReader Instance = new FetchedListensReader();

    protected override FetchedListens ReadPayload(ref Utf8JsonReader reader, JsonSerializerOptions options) {
      ushort? count = null;
      IReadOnlyList<IListen>? listens = null;
      string? user = null;
      long? ts = null;
      Dictionary<string, object?>? rest = null;
      while (reader.TokenType == JsonTokenType.PropertyName) {
        var prop = reader.GetString();
        try {
          reader.Read();
          switch (prop) {
            case "count":
              count = reader.GetUInt16();
              break;
            case "listens":
              listens = JsonUtils.ReadList<IListen, Listen>(ref reader, options, ListenReader.Instance);
              break;
            case "latest_listen_ts":
              ts = reader.GetInt64();
              break;
            case "user_id":
              user = reader.GetString();
              break;
            default:
              rest ??= new Dictionary<string, object?>();
              rest[prop] = AnyObjectReader.Instance.Read(ref reader, typeof(object), options);
              break;
          }
        }
        catch (Exception e) {
          throw new JsonException($"Failed to deserialize the '{prop}' property.", e);
        }
        reader.Read();
      }
      listens = this.VerifyListens(count, listens);
      if (user == null)
        throw new JsonException("Expected user id not found or null.");
      if (ts == null)
        throw new JsonException("Expected latest-listen timestamp not found or null.");
      return new FetchedListens(listens, ts.Value, user) {
        UnhandledProperties = rest
      };
    }

  }

}
