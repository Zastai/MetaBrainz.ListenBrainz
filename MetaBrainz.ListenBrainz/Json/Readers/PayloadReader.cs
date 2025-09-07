using System.Text.Json;

using MetaBrainz.Common.Json.Converters;

namespace MetaBrainz.ListenBrainz.Json.Readers;

internal abstract class PayloadReader<T> : ObjectReader<T> {

  protected sealed override T ReadObjectContents(ref Utf8JsonReader reader, JsonSerializerOptions options) {
    if (reader.TokenType != JsonTokenType.PropertyName || reader.GetString() != "payload") {
      throw new JsonException("Expected payload property not found.");
    }
    reader.Read();
    if (reader.TokenType != JsonTokenType.StartObject) {
      throw new JsonException("Expected start of payload object not found.");
    }
    reader.Read();
    var payload = this.ReadPayload(ref reader, options);
    if (reader.TokenType != JsonTokenType.EndObject) {
      throw new JsonException("Expected end of payload object not found.");
    }
    reader.Read();
    return payload;
  }

  protected abstract T ReadPayload(ref Utf8JsonReader reader, JsonSerializerOptions options);

}
