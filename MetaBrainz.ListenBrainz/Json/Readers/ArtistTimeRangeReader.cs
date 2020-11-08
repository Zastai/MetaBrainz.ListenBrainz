using System;
using System.Collections.Generic;
using System.Text.Json;

using MetaBrainz.Common.Json;
using MetaBrainz.Common.Json.Converters;
using MetaBrainz.ListenBrainz.Interfaces;
using MetaBrainz.ListenBrainz.Objects;

namespace MetaBrainz.ListenBrainz.Json.Readers {

  internal sealed class ArtistTimeRangeReader : ObjectReader<ArtistTimeRange> {

    public static readonly ArtistTimeRangeReader Instance = new ArtistTimeRangeReader();

    protected override ArtistTimeRange ReadObjectContents(ref Utf8JsonReader reader, JsonSerializerOptions options) {
      IReadOnlyList<IArtistInfo>? artists = null;
      string? description = null;
      DateTimeOffset? rangeEnd = null;
      DateTimeOffset? rangeStart = null;
      Dictionary<string, object?>? rest = null;
      while (reader.TokenType == JsonTokenType.PropertyName) {
        var prop = reader.GetString();
        try {
          reader.Read();
          switch (prop) {
            case "artists":
              artists = reader.ReadList(ArtistInfoReader.Instance, options);
              break;
            case "from_ts":
              rangeStart = UnixTime.Convert(reader.GetOptionalInt64());
              break;
            case "time_range":
              description = reader.GetString();
              break;
            case "to_ts":
              rangeEnd = UnixTime.Convert(reader.GetOptionalInt64());
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
      if (description == null)
        throw new JsonException("Expected description not found or null.");
      return new ArtistTimeRange(description) {
        Artists = artists,
        RangeEnd = rangeEnd,
        RangeStart = rangeStart,
        UnhandledProperties = rest,
      };
    }

  }

}
