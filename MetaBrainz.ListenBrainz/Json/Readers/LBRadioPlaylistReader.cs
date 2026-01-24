using System;
using System.Collections.Generic;
using System.Text.Json;

using MetaBrainz.Common.Json;
using MetaBrainz.ListenBrainz.Interfaces.JSPF;
using MetaBrainz.ListenBrainz.Json.Readers.JSPF;
using MetaBrainz.ListenBrainz.Objects;

namespace MetaBrainz.ListenBrainz.Json.Readers;

internal sealed class LBRadioPlaylistReader : PayloadReader<LBRadioPlaylist> {

  public static readonly LBRadioPlaylistReader Instance = new();

  protected override LBRadioPlaylist ReadPayload(ref Utf8JsonReader reader, JsonSerializerOptions options) {
    IReadOnlyList<string>? feedback = null;
    IPlaylist? playlist = null;
    Dictionary<string, object?>? rest = null;
    while (reader.TokenType == JsonTokenType.PropertyName) {
      var prop = reader.GetPropertyName();
      try {
        reader.Read();
        switch (prop) {
          case "feedback":
            feedback = reader.ReadList<string>(options);
            break;
          case "jspf":
            playlist = reader.GetObject(PlaylistReader.Instance, options);
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
    return new LBRadioPlaylist {
      Feedback = feedback ?? throw new MissingField("feedback"),
      Playlist = playlist ?? throw new MissingField("jspf"),
      UnhandledProperties = rest,
    };
  }

}
