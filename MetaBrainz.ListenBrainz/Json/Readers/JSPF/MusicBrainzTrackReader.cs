using System;
using System.Collections.Generic;
using System.Text.Json;

using MetaBrainz.Common.Json;
using MetaBrainz.Common.Json.Converters;
using MetaBrainz.ListenBrainz.Interfaces;
using MetaBrainz.ListenBrainz.Objects.JSPF;

namespace MetaBrainz.ListenBrainz.Json.Readers.JSPF;

internal class MusicBrainzTrackReader : ObjectReader<MusicBrainzTrack> {

  public static readonly MusicBrainzTrackReader Instance = new();

  protected override MusicBrainzTrack ReadObjectContents(ref Utf8JsonReader reader, JsonSerializerOptions options) {
    DateTimeOffset? added = null;
    string? addedBy = null;
    Dictionary<string, object?>? additionalMetadata = null;
    IReadOnlyList<Uri>? artistIds = null;
    long? caaId = null;
    Guid? caaRelease = null;
    IReadOnlyList<IArtistCredit>? credits = null;
    Uri? releaseId = null;
    Dictionary<string, object?>? rest = null;
    while (reader.TokenType == JsonTokenType.PropertyName) {
      var prop = reader.GetPropertyName();
      try {
        reader.Read();
        switch (prop) {
          case "added_at":
            added = reader.GetOptionalDateTimeOffset();
            break;
          case "added_by":
            addedBy = reader.GetString();
            break;
          case "additional_metadata":
            if (reader.TokenType == JsonTokenType.Null) {
              additionalMetadata = null;
            }
            else if (reader.TokenType == JsonTokenType.StartObject) {
              reader.Read();
              while (reader.TokenType == JsonTokenType.PropertyName) {
                var key = reader.GetPropertyName();
                reader.Read();
                try {
                  switch (key) {
                    case "artists":
                      credits = reader.ReadList(ArtistCreditReader.Instance, options);
                      break;
                    case "caa_id":
                      caaId = reader.GetOptionalInt64();
                      break;
                    case "caa_release_mbid":
                      caaRelease = reader.GetOptionalGuid();
                      break;
                    default:
                      // not a well-known value, so store it generically
                      additionalMetadata ??= [];
                      additionalMetadata.Add(key, reader.GetOptionalObject(AnyObjectReader.Instance, options));
                      break;
                  }
                }
                catch (Exception e) {
                  throw new JsonException($"Failed to deserialize additional metadata item '{key}'.", e);
                }
                reader.Read();
              }
              if (reader.TokenType != JsonTokenType.EndObject) {
                throw new JsonException("Expected end of dictionary not found.");
              }
            }
            else {
              throw new JsonException("Expected start of dictionary not found.");
            }
            break;
          case "artist_identifiers":
            artistIds = reader.ReadList<Uri>(options);
            break;
          case "release_identifier":
            releaseId = reader.GetOptionalUri();
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
    return new MusicBrainzTrack {
      Added = added,
      AddedBy = addedBy,
      AdditionalMetadata = additionalMetadata,
      ArtistIds = artistIds,
      CoverArtId = caaId,
      CoverArtReleaseId = caaRelease,
      Credits = credits,
      ReleaseId = releaseId,
      UnhandledProperties = rest,
    };
  }

}
