using System;
using System.Collections.Generic;
using System.Text.Json;

using MetaBrainz.Common.Json;
using MetaBrainz.Common.Json.Converters;
using MetaBrainz.ListenBrainz.Objects;

namespace MetaBrainz.ListenBrainz.Json.Readers;

internal sealed class RecordingFeedbackReader : ObjectReader<RecordingFeedback> {

  public static readonly RecordingFeedbackReader Instance = new();

  protected override RecordingFeedback ReadObjectContents(ref Utf8JsonReader reader, JsonSerializerOptions options) {
    long? created = null;
    Guid? mbid = null;
    Guid? msid = null;
    int? score = null;
    object? trackMetadata = null;
    string? user = null;
    Dictionary<string, object?>? rest = null;
    while (reader.TokenType == JsonTokenType.PropertyName) {
      var prop = reader.GetPropertyName();
      try {
        reader.Read();
        switch (prop) {
          case "created":
            created = reader.GetOptionalInt64();
            break;
          case "recording_mbid":
            mbid = reader.GetOptionalGuid();
            break;
          case "recording_msid":
            msid = reader.GetOptionalGuid();
            break;
          case "score":
            score = reader.GetOptionalInt32();
            break;
          case "track_metadata":
            trackMetadata = reader.GetOptionalObject(options);
            break;
          case "user_id":
            user = reader.GetString();
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
    if (created is null) {
      throw new JsonException("Expected created timestamp not found or null.");
    }
    return new RecordingFeedback {
      Created = DateTimeOffset.FromUnixTimeSeconds(created.Value),
      Id = mbid,
      MessyId = msid,
      Score = score ?? throw new JsonException("Expected score not found or null."),
      TrackMetadata = trackMetadata,
      UnhandledProperties = rest,
      User = user ?? throw new JsonException("Expected user name not found or null."),
    };
  }

}
