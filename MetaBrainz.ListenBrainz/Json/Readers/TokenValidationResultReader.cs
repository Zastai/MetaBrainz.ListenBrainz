using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;

using MetaBrainz.Common.Json;
using MetaBrainz.Common.Json.Converters;
using MetaBrainz.ListenBrainz.Objects;

namespace MetaBrainz.ListenBrainz.Json.Readers;

internal sealed class TokenValidationResultReader : ObjectReader<TokenValidationResult> {

  public static readonly TokenValidationResultReader Instance = new();

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
    return new TokenValidationResult {
      Code = (HttpStatusCode) (code ?? throw new JsonException("Expected status code not found or null.")),
      Message = message ?? throw new JsonException("Expected message not found or null."),
      UnhandledProperties = rest,
      User = user,
      Valid = valid,
    };
  }

}
