using System;
using System.Collections.Generic;
using System.Text.Json;

using MetaBrainz.Common.Json;
using MetaBrainz.Common.Json.Converters;
using MetaBrainz.ListenBrainz.Objects;

namespace MetaBrainz.ListenBrainz.Json.Readers;

internal class ArtistCreditReader : ObjectReader<ArtistCredit> {

  public static readonly ArtistCreditReader Instance = new();

  protected override ArtistCredit ReadObjectContents(ref Utf8JsonReader reader, JsonSerializerOptions options) {
    string? creditedName = null;
    string? joinPhrase = null;
    Guid? id = null;
    Dictionary<string, object?>? rest = null;
    while (reader.TokenType == JsonTokenType.PropertyName) {
      var prop = reader.GetPropertyName();
      try {
        reader.Read();
        switch (prop) {
          case "artist_credit_name":
            creditedName = reader.GetString();
            break;
          case "artist_mbid":
            id = reader.GetOptionalGuid();
            break;
          case "join_phrase":
            joinPhrase = reader.GetString();
            break;
          default:
            rest ??= [ ];
            rest[prop] = reader.GetOptionalObject(options);
            break;
        }
      }
      catch (Exception e) {
        throw new JsonException($"Failed to deserialize the '{prop}' property.", e);
      }
      reader.Read();
    }
    return new ArtistCredit {
      CreditedName = creditedName ?? throw new JsonException("Expected artist credit name not found or null."),
      Id = id ?? throw new JsonException("Expected artist ID not found or null."),
      JoinPhrase = joinPhrase ?? throw new JsonException("Expected join phrase not found or null."),
      UnhandledProperties = rest,
    };
  }

}
