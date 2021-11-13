using System;
using System.Collections.Generic;
using System.Text.Json;

using MetaBrainz.Common.Json;
using MetaBrainz.Common.Json.Converters;
using MetaBrainz.ListenBrainz.Interfaces;
using MetaBrainz.ListenBrainz.Objects;

namespace MetaBrainz.ListenBrainz.Json.Readers;

internal sealed class PlayingTrackReader : ObjectReader<PlayingTrack> {

  public static readonly PlayingTrackReader Instance = new PlayingTrackReader();

  protected override PlayingTrack ReadObjectContents(ref Utf8JsonReader reader, JsonSerializerOptions options) {
    ITrackInfo? track = null;
    Dictionary<string, object?>? rest = null;
    while (reader.TokenType == JsonTokenType.PropertyName) {
      var prop = reader.GetPropertyName();
      try {
        reader.Read();
        switch (prop) {
          case "track_metadata":
            track = reader.GetObject(TrackInfoReader.Instance, options);
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
    if (track == null) {
      throw new JsonException("Required track metadata not found.");
    }
    return new PlayingTrack(track) {
      UnhandledProperties = rest
    };
  }

}
