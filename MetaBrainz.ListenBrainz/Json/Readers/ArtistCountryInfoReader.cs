using System;
using System.Collections.Generic;
using System.Text.Json;

using MetaBrainz.Common.Json;
using MetaBrainz.Common.Json.Converters;
using MetaBrainz.ListenBrainz.Interfaces;
using MetaBrainz.ListenBrainz.Objects;

namespace MetaBrainz.ListenBrainz.Json.Readers;

internal class ArtistCountryInfoReader : ObjectReader<ArtistCountryInfo> {

  public static readonly ArtistCountryInfoReader Instance = new();

  protected override ArtistCountryInfo ReadObjectContents(ref Utf8JsonReader reader, JsonSerializerOptions options) {
    int? artistCount = null;
    IReadOnlyList<IArtistInfo>? artists = null;
    int? listenCount = null;
    string? country = null;
    Dictionary<string, object?>? rest = null;
    while (reader.TokenType == JsonTokenType.PropertyName) {
      var prop = reader.GetPropertyName();
      try {
        reader.Read();
        switch (prop) {
          case "artists":
            artists = reader.ReadList(ArtistInfoReader.Instance, options);
            break;
          case "artist_count":
            artistCount = reader.GetInt32();
            break;
          case "country":
            country = reader.GetString();
            break;
          case "listen_count":
            listenCount = reader.GetInt32();
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
    return new ArtistCountryInfo {
      ArtistCount = artistCount ?? throw new JsonException("Expected artist count not found or null."),
      Artists = artists,
      Country = country ?? throw new JsonException("Expected country code not found or null."),
      ListenCount = listenCount ?? throw new JsonException("Expected listen count not found or null."),
      UnhandledProperties = rest,
    };
  }

}
