using System.Text.Json;

using MetaBrainz.Common.Json;
using MetaBrainz.Common.Json.Converters;
using MetaBrainz.ListenBrainz.Objects.JSPF;

namespace MetaBrainz.ListenBrainz.Json.Readers.JSPF;

internal sealed class NamedUriReader : ObjectReader<NamedUri> {

  public static readonly NamedUriReader Instance = new();

  protected override NamedUri ReadObjectContents(ref Utf8JsonReader reader, JsonSerializerOptions options) {
    // This is expected to have a single property, where the property name is the name and the value is the URI.
    if (reader.TokenType != JsonTokenType.PropertyName) {
      throw new JsonException("The (single) property is missing.");
    }
    var name = reader.GetPropertyName();
    reader.Read();
    var uri = reader.GetUri();
    if (reader.TokenType != JsonTokenType.EndObject) {
      throw new JsonException("Excess contents encountered.");
    }
    return new NamedUri { Name = name, Uri = uri, };
  }

}
