using System;
using System.Collections.Generic;
using System.Text.Json;

using MetaBrainz.Common.Json;
using MetaBrainz.Common.Json.Converters;
using MetaBrainz.ListenBrainz.Objects;

namespace MetaBrainz.ListenBrainz.Json.Readers;

internal sealed class LatestImportReader : ObjectReader<LatestImport> {

  public static readonly LatestImportReader Instance = new();

  protected override LatestImport ReadObjectContents(ref Utf8JsonReader reader, JsonSerializerOptions options) {
    long? ts = null;
    string? user = null;
    Dictionary<string, object?>? rest = null;
    while (reader.TokenType == JsonTokenType.PropertyName) {
      var prop = reader.GetPropertyName();
      try {
        reader.Read();
        switch (prop) {
          case "latest_import":
            ts = reader.GetInt64();
            break;
          case "musicbrainz_id":
            user = reader.GetString();
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
    if (ts is null) {
      throw new JsonException("Expected latest-import timestamp not found or null.");
    }
    if (user is null) {
      throw new JsonException("Expected user id not found or null.");
    }
    return new LatestImport(ts.Value, user) {
      UnhandledProperties = rest
    };
  }

}
