using System;
using System.Collections.Generic;
using System.Text.Json;

using MetaBrainz.Common.Json.Converters;
using MetaBrainz.ListenBrainz.Objects;

namespace MetaBrainz.ListenBrainz.Json.Readers {

  internal sealed class AdditionalInfoReader : ObjectReader<AdditionalInfo> {

    public static readonly AdditionalInfoReader Instance = new AdditionalInfoReader();

    protected override AdditionalInfo ReadObjectContents(ref Utf8JsonReader reader, JsonSerializerOptions options) {
      var fields = new Dictionary<string, object?>();
      while (reader.TokenType == JsonTokenType.PropertyName) {
        var prop = reader.GetString();
        try {
          reader.Read();
          // There are no guaranteed contents, and no required property types, so nothing specific to do here (yet).
          fields[prop] = JsonSerializer.Deserialize<object>(ref reader, options);
        }
        catch (Exception e) {
          throw new JsonException($"Failed to deserialize the '{prop}' property.", e);
        }
        reader.Read();
      }
      return new AdditionalInfo(fields);
    }

  }

}
