using System;
using System.Collections.Generic;
using System.Text.Json;

using MetaBrainz.Common.Json.Converters;
using MetaBrainz.ListenBrainz.Objects;

namespace MetaBrainz.ListenBrainz.Json.Readers {

  internal sealed class ErrorInfoReader : ObjectReader<ErrorInfo> {

    public static readonly ErrorInfoReader Instance = new ErrorInfoReader();

    protected override ErrorInfo ReadObjectContents(ref Utf8JsonReader reader, JsonSerializerOptions options) {
      int? code = null;
      string? error = null;
      Dictionary<string, object?>? rest = null;
      while (reader.TokenType == JsonTokenType.PropertyName) {
        var prop = reader.GetString();
        try {
          reader.Read();
          switch (prop) {
            case "code":
              code = reader.GetInt32();
              break;
            case "error":
              error = reader.GetString();
              break;
            default:
              rest ??= new Dictionary<string, object?>();
              rest[prop] = AnyObjectReader.Instance.Read(ref reader, typeof(object), options);
              break;
          }
        }
        catch (Exception e) {
          throw new JsonException($"Failed to deserialize the '{prop}' property.", e);
        }
        reader.Read();
      }
      if (!code.HasValue)
        throw new JsonException("Expected error code not found or null.");
      if (error == null)
        throw new JsonException("Expected error message not found or null.");
      return new ErrorInfo(code.Value, error) {
        UnhandledProperties = rest
      };
    }

  }

}
