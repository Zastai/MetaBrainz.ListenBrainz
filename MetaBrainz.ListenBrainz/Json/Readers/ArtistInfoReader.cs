using System;
using System.Collections.Generic;
using System.Text.Json;

using MetaBrainz.Common.Json;
using MetaBrainz.Common.Json.Converters;
using MetaBrainz.ListenBrainz.Objects;

namespace MetaBrainz.ListenBrainz.Json.Readers;

internal class ArtistInfoReader : ObjectReader<ArtistInfo> {

  public static readonly ArtistInfoReader Instance = new();

  protected override ArtistInfo ReadObjectContents(ref Utf8JsonReader reader, JsonSerializerOptions options) {
    int? listenCount = null;
    IReadOnlyList<Guid>? mbids = null;
    Guid? msid = null;
    string? name = null;
    Dictionary<string, object?>? rest = null;
    while (reader.TokenType == JsonTokenType.PropertyName) {
      var prop = reader.GetPropertyName();
      try {
        reader.Read();
        switch (prop) {
          case "artist_mbids":
            mbids = reader.ReadList<Guid>(options);
            break;
          case "artist_msid":
            msid = reader.GetOptionalGuid();
            break;
          case "artist_name":
            name = reader.GetString();
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
    if (listenCount == null) {
      throw new JsonException("Expected listen count not found or null.");
    }
    if (name == null) {
      throw new JsonException("Expected artist name not found or null.");
    }
    return new ArtistInfo(name, listenCount.Value) {
      Ids = mbids,
      MessyId = msid,
      UnhandledProperties = rest,
    };
  }

}
