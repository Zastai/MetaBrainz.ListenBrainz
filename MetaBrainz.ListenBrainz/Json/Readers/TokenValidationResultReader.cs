using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;

using MetaBrainz.Common.Json;
using MetaBrainz.Common.Json.Converters;
using MetaBrainz.ListenBrainz.Objects;

namespace MetaBrainz.ListenBrainz.Json.Readers;

internal sealed class TokenValidationResultReader : ObjectReader<TokenValidationResult> {

  public static readonly TokenValidationResultReader Instance = new TokenValidationResultReader();

  protected override TokenValidationResult ReadObjectContents(ref Utf8JsonReader reader, JsonSerializerOptions options) {
    short? code = null;
    string? message = null;
    string? user = null;
    bool? valid = null;
    Dictionary<string, object?>? rest = null;
    while (reader.TokenType == JsonTokenType.PropertyName) {
      var prop = reader.GetPropertyName();
      try {
        reader.Read();
        switch (prop) {
          case "code":
            code = reader.GetInt16();
            break;
          case "message":
            message = reader.GetString();
            break;
          case "user_name":
            user = reader.GetString();
            break;
          case "valid":
            valid = reader.GetBoolean();
            break;
          default:
            rest ??= new Dictionary<string, object?>();
            rest[prop] = reader.GetOptionalObject(options);
            break;
        }
      }
      catch (Exception e) {
        throw new JsonException($"Failed to deserialize the '{prop}' property.", e);
      }
      reader.Read();
    }
    if (code == null) {
      throw new JsonException("Expected status code not found or null.");
    }
    if (message == null) {
      throw new JsonException("Expected message not found or null.");
    }
    return new TokenValidationResult((HttpStatusCode) code.Value, message) {
      UnhandledProperties = rest,
      User = user,
      Valid = valid,
    };
  }

}
