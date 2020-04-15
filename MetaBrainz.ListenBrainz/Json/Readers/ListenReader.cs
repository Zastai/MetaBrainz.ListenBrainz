using System;
using System.Collections.Generic;
using System.Text.Json;

using MetaBrainz.Common.Json.Converters;
using MetaBrainz.ListenBrainz.Interfaces;
using MetaBrainz.ListenBrainz.Objects;

namespace MetaBrainz.ListenBrainz.Json.Readers {

  internal class ListenReader : ObjectReader<Listen> {

    public static readonly ListenReader Instance = new ListenReader();

    protected override Listen ReadObjectContents(ref Utf8JsonReader reader, JsonSerializerOptions options) {
      Guid? msid = null;
      ITrackInfo? track = null;
      string? user = null;
      long? ts = null;
      Dictionary<string, object?>? rest = null;
      while (reader.TokenType == JsonTokenType.PropertyName) {
        var prop = reader.GetString();
        try {
          reader.Read();
          switch (prop) {
            case "listened_at":
              ts = reader.GetInt64();
              break;
            case "recording_msid":
              msid = reader.GetGuid();
              break;
            case "track_metadata":
              track = TrackInfoReader.Instance.Read(ref reader, typeof(TrackInfo), options);
              break;
            case "user_name":
              user = reader.GetString();
              break;
            default:
              rest ??= new Dictionary<string, object?>();
              rest[prop] = JsonSerializer.Deserialize<object>(ref reader, options);
              break;
          }
        }
        catch (Exception e) {
          throw new JsonException($"Failed to deserialize the '{prop}' property.", e);
        }
        reader.Read();
      }
      if (msid == null)
        throw new JsonException("Expected MessyBrainz recording id not found or null.");
      if (track == null)
        throw new JsonException("Expected track metadata not found or null.");
      if (user == null)
        throw new JsonException("Expected user name not found or null.");
      if (ts == null)
        throw new JsonException("Expected listened-at timestamp not found or null.");
      return new Listen(msid.Value, ts.Value, track, user) {
        UnhandledProperties = rest
      };
    }

  }

}
