using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;

using MetaBrainz.Common.Json;
using MetaBrainz.Common.Json.Converters;
using MetaBrainz.ListenBrainz.Interfaces;
using MetaBrainz.ListenBrainz.Objects;

namespace MetaBrainz.ListenBrainz.Json.Readers;

internal class NewReleaseReader : ObjectReader<NewRelease> {

  public static readonly NewReleaseReader Instance = new();


  protected override NewRelease ReadObjectContents(ref Utf8JsonReader reader, JsonSerializerOptions options) {
    string? artist = null;
    long? caaId = null;
    Guid? caaRelease = null;
    IReadOnlyList<Guid>? creditIds = null;
    IReadOnlyList<string>? creditNames = null;
    IReadOnlyList<IArtistCredit>? credits = null;
    string? firstReleaseDate = null;
    Guid? releaseGroupId = null;
    Guid? releaseId = null;
    string? title = null;
    string? type = null;
    Dictionary<string, object?>? rest = null;
    while (reader.TokenType == JsonTokenType.PropertyName) {
      var prop = reader.GetPropertyName();
      try {
        reader.Read();
        switch (prop) {
          case "artist_credit_mbids":
            creditIds = reader.ReadList<Guid>(options);
            break;
          case "artist_credit_name":
            artist = reader.GetString();
            break;
          case "artist_credit_names":
            creditNames = reader.ReadList<string>(options);
            break;
          case "artists":
            credits = reader.ReadList(ArtistCreditReader.Instance, options);
            break;
          case "caa_id":
            caaId = reader.GetOptionalInt64();
            break;
          case "caa_release_mbid":
            caaRelease = reader.GetOptionalGuid();
            break;
          case "first_release_date":
            firstReleaseDate = reader.GetString();
            break;
          case "release_mbid":
            releaseId = reader.GetOptionalGuid();
            break;
          case "release_group_mbid":
            releaseGroupId = reader.GetOptionalGuid();
            break;
          case "title":
            title = reader.GetString();
            break;
          case "type":
            type = reader.GetString();
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
    return new NewRelease {
      CoverArtId = caaId,
      CoverArtReleaseId = caaRelease,
      CreditedArtist = artist,
      CreditedArtistIds = creditIds,
      CreditedArtistNames = creditNames,
      CreditedArtists = credits,
      FirstReleaseDate = firstReleaseDate,
      ReleaseGroupId = releaseGroupId,
      ReleaseId = releaseId,
      Title = title ?? throw new JsonException("Expected release title not found or null."),
      Type = type,
      UnhandledProperties = rest,
    };
  }

}
