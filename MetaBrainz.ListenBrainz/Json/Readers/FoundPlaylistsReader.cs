using System;
using System.Collections.Generic;
using System.Text.Json;

using MetaBrainz.Common.Json;
using MetaBrainz.Common.Json.Converters;
using MetaBrainz.ListenBrainz.Interfaces;
using MetaBrainz.ListenBrainz.Interfaces.JSPF;
using MetaBrainz.ListenBrainz.Json.Readers.JSPF;
using MetaBrainz.ListenBrainz.Objects;

namespace MetaBrainz.ListenBrainz.Json.Readers;

internal sealed class FoundPlaylistsReader : ObjectReader<FoundPlaylists> {

  public static readonly FoundPlaylistsReader Instance = new();

  protected override FoundPlaylists ReadObjectContents(ref Utf8JsonReader reader, JsonSerializerOptions options)  {
    int? count = null;
    int? offset = null;
    IReadOnlyList<IPlaylist>? playlists = null;
    int? totalCount = null;
    Dictionary<string, object?>? rest = null;
    while (reader.TokenType == JsonTokenType.PropertyName) {
      var prop = reader.GetPropertyName();
      try {
        reader.Read();
        switch (prop) {
          case "count":
            count = reader.GetInt32();
            break;
          case "offset":
            offset = reader.GetInt32();
            break;
          case "playlist_count":
            totalCount = reader.GetInt32();
            break;
          case "playlists":
            playlists = reader.ReadList(PlaylistReader.Instance, options);
            break;
          default:
            rest ??= [ ];
            rest[prop] = reader.GetOptionalObject(options);
            break;
        }
      }
      catch (Exception e) {
        throw new JsonException($"Failed to deserialize the '{prop}' property.", e);
      }
      reader.Read();
    }
    return new FoundPlaylists {
      Count = count ?? throw new MissingField("count"),
      Offset = offset ?? throw new MissingField("offset"),
      Playlists = playlists ?? throw new MissingField("playlists"),
      TotalCount = totalCount ?? throw new MissingField("playlist_count"),
      UnhandledProperties = rest,
    };
  }

}
