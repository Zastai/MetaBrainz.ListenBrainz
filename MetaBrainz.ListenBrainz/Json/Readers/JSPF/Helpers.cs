using System;
using System.Collections.Generic;
using System.Text.Json;

using MetaBrainz.Common.Json;
using MetaBrainz.Common.Json.Converters;

namespace MetaBrainz.ListenBrainz.Json.Readers.JSPF;

using ExtensionData = IReadOnlyDictionary<Uri, IReadOnlyList<object?>?>;

internal static class Helpers {

  public static ExtensionData? ReadExtensions(this ref Utf8JsonReader reader, JsonSerializerOptions options) {
    if (reader.TokenType == JsonTokenType.Null) {
      return null;
    }
    if (reader.TokenType != JsonTokenType.StartObject) {
      throw new JsonException("Expected start of extension data not found.");
    }
    reader.Read();
    Dictionary<Uri, IReadOnlyList<object?>?> extensions = [ ];
    while (reader.TokenType == JsonTokenType.PropertyName) {
      var prop = reader.GetPropertyName();
      try {
        if (!Uri.TryCreate(prop, UriKind.Absolute, out var id)) {
          throw new JsonException($"The extension application identifier ({prop}) is not a valid URI.");
        }
        reader.Read();
        if (reader.TokenType == JsonTokenType.StartObject) {
          // LB-1836: this is a violation of the JSPF spec, but it's what ListenBrainz currently does.
          extensions.Add(id, [reader.GetOptionalObject(options)]);
        }
        else {
          extensions.Add(id, reader.ReadList(AnyObjectReader.Instance, options));
        }
      }
      catch (Exception e) {
        throw new JsonException($"Failed to deserialize the data for the '{prop}' extension.", e);
      }
      reader.Read();
    }
    if (reader.TokenType != JsonTokenType.EndObject) {
      throw new JsonException("Expected end of extension data not found.");
    }
    return extensions;
  }

}
