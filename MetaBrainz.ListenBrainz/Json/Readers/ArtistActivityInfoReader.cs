using System;
using System.Collections.Generic;
using System.Text.Json;

using MetaBrainz.Common.Json;
using MetaBrainz.Common.Json.Converters;
using MetaBrainz.ListenBrainz.Interfaces;
using MetaBrainz.ListenBrainz.Objects;

namespace MetaBrainz.ListenBrainz.Json.Readers;

internal class ArtistActivityInfoReader : ObjectReader<ArtistActivityInfo> {

  public static readonly ArtistActivityInfoReader Instance = new();

  protected override ArtistActivityInfo ReadObjectContents(ref Utf8JsonReader reader, JsonSerializerOptions options) {
    IReadOnlyList<IAlbumInfo>? albums = null;
    int? listenCount = null;
    Guid? mbid = null;
    string? name = null;
    string? artistName = null;
    Dictionary<string, object?>? rest = null;
    while (reader.TokenType == JsonTokenType.PropertyName) {
      var prop = reader.GetPropertyName();
      try {
        reader.Read();
        switch (prop) {
          case "albums":
            albums = reader.ReadList(AlbumInfoReader.Instance, options);
            break;
          case "artist_mbid":
            mbid = reader.GetOptionalGuid();
            break;
          case "artist_name":
            name = reader.GetString();
            break;
          case "listen_count":
            listenCount = reader.GetInt32();
            break;
          case "name":
            name = reader.GetString();
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
    // Retain artist_name (as unhandled) only if it's not a duplicate of the name
    if (artistName is not null && artistName != name) {
      rest ??= new Dictionary<string, object?>();
      rest["artist_name"] = artistName;
    }
    return new ArtistActivityInfo {
      Albums = albums ?? [ ],
      Id = mbid,
      ListenCount = listenCount ?? throw new JsonException("Expected listen count not found or null."),
      Name = name ?? throw new JsonException("Expected artist name not found or null."),
      UnhandledProperties = rest,
    };
  }

}
