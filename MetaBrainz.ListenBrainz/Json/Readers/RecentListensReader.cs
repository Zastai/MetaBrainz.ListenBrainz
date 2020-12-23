using System;
using System.Collections.Generic;
using System.Text.Json;

using MetaBrainz.Common.Json;
using MetaBrainz.ListenBrainz.Interfaces;
using MetaBrainz.ListenBrainz.Objects;

namespace MetaBrainz.ListenBrainz.Json.Readers {

  internal sealed class RecentListensReader : PayloadReader<RecentListens> {

    public static readonly RecentListensReader Instance = new RecentListensReader();

    protected override RecentListens ReadPayload(ref Utf8JsonReader reader, JsonSerializerOptions options) {
      int? count = null;
      IReadOnlyList<IListen>? listens = null;
      string? userList = null;
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
            case "user_list":
              userList = reader.GetString();
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
      if (userList == null)
        throw new JsonException("Expected user list not found or null.");
      return new RecentListens(listens, userList) {
        UnhandledProperties = rest
      };
    }

  }
}
