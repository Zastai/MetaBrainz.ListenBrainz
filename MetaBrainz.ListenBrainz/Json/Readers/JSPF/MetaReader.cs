using System;
using System.Text.Json;

using MetaBrainz.Common.Json;
using MetaBrainz.Common.Json.Converters;
using MetaBrainz.ListenBrainz.Objects.JSPF;

namespace MetaBrainz.ListenBrainz.Json.Readers.JSPF;

internal sealed class MetaReader : ObjectReader<Meta> {

  public static readonly MetaReader Instance = new();

  protected override Meta ReadObjectContents(ref Utf8JsonReader reader, JsonSerializerOptions options) {
    // This is expected to have a single property, where the name is the metadata type URI and the value is the plain text metadata.
    if (reader.TokenType != JsonTokenType.PropertyName) {
      throw new JsonException("The (single) property is missing.");
    }
    var prop = reader.GetPropertyName();
    if (!Uri.TryCreate(prop, UriKind.Absolute, out var id)) {
      throw new JsonException($"The link identifier ({prop}) is not a valid URI.");
    }
    reader.Read();
    var value = reader.GetString() ?? "";
    if (reader.TokenType != JsonTokenType.EndObject) {
      throw new JsonException("Excess contents encountered.");
    }
    return new Meta { Id = id, Value = value, };
  }

}
