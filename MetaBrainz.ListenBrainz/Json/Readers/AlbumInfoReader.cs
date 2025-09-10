using System;
using System.Collections.Generic;
using System.Text.Json;

using MetaBrainz.Common.Json;
using MetaBrainz.Common.Json.Converters;
using MetaBrainz.ListenBrainz.Objects;

namespace MetaBrainz.ListenBrainz.Json.Readers;

internal class AlbumInfoReader : ObjectReader<AlbumInfo> {

  public static readonly AlbumInfoReader Instance = new();

  protected override AlbumInfo ReadObjectContents(ref Utf8JsonReader reader, JsonSerializerOptions options) {
    int? listenCount = null;
    Guid? mbid = null;
    string? title = null;
    Dictionary<string, object?>? rest = null;
    while (reader.TokenType == JsonTokenType.PropertyName) {
      var prop = reader.GetPropertyName();
      try {
        reader.Read();
        switch (prop) {
          case "listen_count":
            listenCount = reader.GetInt32();
            break;
          case "name":
            title = reader.GetString();
            break;
          case "release_group_mbid":
            mbid = reader.GetOptionalGuid();
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
    return new AlbumInfo {
      Id = mbid,
      ListenCount = listenCount ?? throw new JsonException("Expected listen count not found or null."),
      Title = title ?? throw new JsonException("Expected album name not found or null."),
      UnhandledProperties = rest,
    };
  }

}
