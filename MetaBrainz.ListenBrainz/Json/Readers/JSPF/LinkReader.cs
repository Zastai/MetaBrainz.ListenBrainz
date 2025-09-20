using System;
using System.Text.Json;

using MetaBrainz.Common.Json;
using MetaBrainz.Common.Json.Converters;
using MetaBrainz.ListenBrainz.Objects.JSPF;

namespace MetaBrainz.ListenBrainz.Json.Readers.JSPF;

internal sealed class LinkReader : ObjectReader<Link> {

  public static readonly LinkReader Instance = new();

  protected override Link ReadObjectContents(ref Utf8JsonReader reader, JsonSerializerOptions options) {
    // This is expected to have a single property, where the name is the resource type URI and the value is the resource URI.
    if (reader.TokenType != JsonTokenType.PropertyName) {
      throw new JsonException("The (single) property is missing.");
    }
    var prop = reader.GetPropertyName();
    if (!Uri.TryCreate(prop, UriKind.Absolute, out var id)) {
      throw new JsonException($"The link identifier ({prop}) is not a valid URI.");
    }
    reader.Read();
    var value = reader.GetUri();
    if (reader.TokenType != JsonTokenType.EndObject) {
      throw new JsonException("Excess contents encountered.");
    }
    return new Link { Id = id, Value = value, };
  }

}
