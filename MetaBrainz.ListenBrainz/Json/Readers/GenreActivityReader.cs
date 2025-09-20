using System;
using System.Collections.Generic;
using System.Text.Json;

using MetaBrainz.Common.Json;
using MetaBrainz.Common.Json.Converters;
using MetaBrainz.ListenBrainz.Interfaces;
using MetaBrainz.ListenBrainz.Objects;

namespace MetaBrainz.ListenBrainz.Json.Readers;

internal sealed class GenreActivityReader : ObjectReader<GenreActivity> {

  public static readonly GenreActivityReader Instance = new();

  protected override GenreActivity ReadObjectContents(ref Utf8JsonReader reader, JsonSerializerOptions options) {
    IReadOnlyList<IGenreActivityDetails>? activity = null;
    Dictionary<string, object?>? rest = null;
    while (reader.TokenType == JsonTokenType.PropertyName) {
      var prop = reader.GetPropertyName();
      try {
        reader.Read();
        switch (prop) {
          case "result":
            activity = reader.ReadList(GenreActivityDetailsReader.Instance, options);
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
    if (activity is null) {
      throw new JsonException("Expected result set not found or null.");
    }
    return new GenreActivity {
      Activity = activity,
    };
  }

}
