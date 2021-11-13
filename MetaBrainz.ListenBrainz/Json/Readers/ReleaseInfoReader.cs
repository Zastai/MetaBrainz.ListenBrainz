using System;
using System.Collections.Generic;
using System.Text.Json;

using MetaBrainz.Common.Json;
using MetaBrainz.Common.Json.Converters;
using MetaBrainz.ListenBrainz.Objects;

namespace MetaBrainz.ListenBrainz.Json.Readers;

internal class ReleaseInfoReader : ObjectReader<ReleaseInfo> {

  public static readonly ReleaseInfoReader Instance = new ReleaseInfoReader();

  protected override ReleaseInfo ReadObjectContents(ref Utf8JsonReader reader, JsonSerializerOptions options) {
    IReadOnlyList<Guid>? artistMbids = null;
    Guid? artistMsid = null;
    string? artistName = null;
    int? listenCount = null;
    Guid? mbid = null;
    Guid? msid = null;
    string? name = null;
    Dictionary<string, object?>? rest = null;
    while (reader.TokenType == JsonTokenType.PropertyName) {
      var prop = reader.GetPropertyName();
      try {
        reader.Read();
        switch (prop) {
          case "artist_mbids":
            artistMbids = reader.ReadList<Guid>(options);
            break;
          case "artist_msid":
            artistMsid = reader.GetOptionalGuid();
            break;
          case "artist_name":
            artistName = reader.GetString();
            break;
          case "listen_count":
            listenCount = reader.GetInt32();
            break;
          case "release_mbid":
            mbid = reader.GetOptionalGuid();
            break;
          case "release_msid":
            msid = reader.GetOptionalGuid();
            break;
          case "release_name":
            name = reader.GetString();
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
      throw new JsonException("Expected release name not found or null.");
    }
    return new ReleaseInfo(name, listenCount.Value) {
      ArtistIds = artistMbids,
      ArtistMessyId = artistMsid,
      ArtistName = artistName,
      Id = mbid,
      MessyId = msid,
      UnhandledProperties = rest,
    };
  }

}
