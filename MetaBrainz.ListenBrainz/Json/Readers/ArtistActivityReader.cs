using System;
using System.Collections.Generic;
using System.Text.Json;

using MetaBrainz.Common.Json;
using MetaBrainz.Common.Json.Converters;
using MetaBrainz.ListenBrainz.Objects;

namespace MetaBrainz.ListenBrainz.Json.Readers;

internal class ArtistActivityReader : ObjectReader<ArtistActivity> {

  public static readonly ArtistActivityReader Instance = new();

  protected override ArtistActivity ReadObjectContents(ref Utf8JsonReader reader, JsonSerializerOptions options) {
    IReadOnlyList<ArtistActivityInfo>? artists = null;
    Dictionary<string, object?>? rest = null;
    while (reader.TokenType == JsonTokenType.PropertyName) {
      var prop = reader.GetPropertyName();
      try {
        reader.Read();
        switch (prop) {
          case "result":
            artists = reader.ReadList(ArtistActivityInfoReader.Instance, options);
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
    return new ArtistActivity {
      Artists = artists ?? throw new JsonException("Expected result set not found or null."),
      UnhandledProperties = rest,
    };
  }

}
