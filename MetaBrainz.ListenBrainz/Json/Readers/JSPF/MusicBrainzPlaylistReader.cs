using System;
using System.Collections.Generic;
using System.Text.Json;

using MetaBrainz.Common.Json;
using MetaBrainz.Common.Json.Converters;
using MetaBrainz.ListenBrainz.Objects.JSPF;

namespace MetaBrainz.ListenBrainz.Json.Readers.JSPF;

internal class MusicBrainzPlaylistReader : ObjectReader<MusicBrainzPlaylist> {

  public static readonly MusicBrainzPlaylistReader Instance = new();

  protected override MusicBrainzPlaylist ReadObjectContents(ref Utf8JsonReader reader, JsonSerializerOptions options) {
    IReadOnlyDictionary<string, object?>? additionalMetadata = null;
    IReadOnlyDictionary<string, object?>? algorithmMetadata = null;
    IReadOnlyList<string>? collaborators = null;
    Uri? copiedFrom = null;
    bool? copiedFromDeleted = null;
    string? createdFor = null;
    string? creator = null;
    bool? isPublic = null;
    DateTimeOffset? lastModified = null;
    Dictionary<string, object?>? rest = null;
    while (reader.TokenType == JsonTokenType.PropertyName) {
      var prop = reader.GetPropertyName();
      try {
        reader.Read();
        switch (prop) {
          case "additional_metadata":
            additionalMetadata = reader.ReadDictionary(OptionalObjectReader.Instance, options);
            break;
          case "algorithm_metadata":
            algorithmMetadata = reader.ReadDictionary(OptionalObjectReader.Instance, options);
            break;
          case "collaborators":
            collaborators = reader.ReadList<string>(options);
            break;
          case "copied_from":
            copiedFrom = reader.GetOptionalUri();
            break;
          case "copied_from_deleted":
            copiedFromDeleted = reader.GetOptionalBoolean();
            break;
          case "created_for":
            createdFor = reader.GetString();
            break;
          case "creator":
            creator = reader.GetString();
            break;
          case "last_modified_at":
            lastModified = reader.GetOptionalDateTimeOffset();
            break;
          case "public":
            isPublic = reader.GetOptionalBoolean();
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
    // The 2021 Year in Music playlists have algorithm_metadata but not additional_metadata. The 2022 editions have
    // algorithm_metadata as a property of additional_metadata. Adjust the 2021 case to look like the 2022 case; but if both
    // metadata fields are present, consider algorithm_metadata to be unhandled.
    if (algorithmMetadata is not null) {
      if (additionalMetadata is null) {
        additionalMetadata = new Dictionary<string, object?> { { "algorithm_metadata", algorithmMetadata } };
      }
      else {
        rest ??= [ ];
        rest["algorithm_metadata"] = algorithmMetadata;
      }
    }
    return new MusicBrainzPlaylist {
      AdditionalMetadata = additionalMetadata,
      Collaborators = collaborators,
      CopiedFrom = copiedFrom,
      CopiedFromDeleted = copiedFromDeleted,
      CreatedFor = createdFor,
      Creator = creator,
      LastModified = lastModified,
      Public = isPublic,
      UnhandledProperties = rest,
    };
  }

}
