using System;
using System.Collections.Generic;
using System.Text.Json;

using MetaBrainz.Common.Json;
using MetaBrainz.Common.Json.Converters;
using MetaBrainz.ListenBrainz.Interfaces;
using MetaBrainz.ListenBrainz.Objects;

namespace MetaBrainz.ListenBrainz.Json.Readers;

internal class ListenReader : ObjectReader<Listen> {

  public static readonly ListenReader Instance = new();

  protected override Listen ReadObjectContents(ref Utf8JsonReader reader, JsonSerializerOptions options) {
    long? inserted = null;
    Guid? msid = null;
    ITrackInfo? track = null;
    string? user = null;
    long? listened = null;
    Dictionary<string, object?>? rest = null;
    while (reader.TokenType == JsonTokenType.PropertyName) {
      var prop = reader.GetPropertyName();
      try {
        reader.Read();
        switch (prop) {
          case "inserted_at":
            inserted = reader.GetOptionalInt64();
            break;
          case "listened_at":
            listened = reader.GetOptionalInt64();
            break;
          case "recording_msid":
            msid = reader.GetGuid();
            break;
          case "track_metadata":
            track = reader.GetObject(TrackInfoReader.Instance, options);
            break;
          case "user_name":
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
    if (inserted is null) {
      throw new JsonException("Expected inserted-at timestamp not found or null.");
    }
    if (listened is null) {
      throw new JsonException("Expected listened-at timestamp not found or null.");
    }
    if (msid is null) {
      throw new JsonException("Expected MessyBrainz recording id not found or null.");
    }
    if (track is null) {
      throw new JsonException("Expected track metadata not found or null.");
    }
    if (user is null) {
      throw new JsonException("Expected user name not found or null.");
    }
    return new Listen(inserted.Value, listened.Value, msid.Value, track, user) {
      UnhandledProperties = rest
    };
  }

}
