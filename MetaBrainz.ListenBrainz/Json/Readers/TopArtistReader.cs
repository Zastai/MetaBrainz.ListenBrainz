using System;
using System.Collections.Generic;
using System.Text.Json;

using MetaBrainz.Common.Json;
using MetaBrainz.Common.Json.Converters;
using MetaBrainz.ListenBrainz.Objects;

namespace MetaBrainz.ListenBrainz.Json.Readers;

internal class TopArtistReader : ObjectReader<TopArtist> {

  public static readonly TopArtistReader Instance = new();

  protected override TopArtist ReadObjectContents(ref Utf8JsonReader reader, JsonSerializerOptions options) {
    int? listenCount = null;
    Guid? mbid = null;
    IReadOnlyList<Guid>? mbids = null;
    string? name = null;
    Dictionary<string, object?>? rest = null;
    while (reader.TokenType == JsonTokenType.PropertyName) {
      var prop = reader.GetPropertyName();
      try {
        reader.Read();
        switch (prop) {
          case "artist_mbid":
            mbid = reader.GetOptionalGuid();
            break;
          case "artist_mbids":
            mbids = reader.ReadList<Guid>(options);
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
    if (mbids is not null) {
      if (mbids.Count == 0) {
        mbids = null;
      }
      else if (mbids.Count == 1 && (mbid is null || mbid.Value == mbids[0])) {
        mbid = mbids[0];
        mbids = null;
      }
    }
    // if we did not use a non-empty artist_mbids to populate artist_mbid, retain it as an unhandled property
    if (mbids is not null) {
      rest ??= new Dictionary<string, object?>();
      rest["artist_mbids"] = mbids;
    }
    return new TopArtist {
      Id = mbid,
      ListenCount = listenCount ?? throw new JsonException("Expected listen count not found or null."),
      Name = name ?? throw new JsonException("Expected artist name not found or null."),
      UnhandledProperties = rest,
    };
  }

}
