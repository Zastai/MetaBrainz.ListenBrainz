using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;

using MetaBrainz.Common.Json;
using MetaBrainz.Common.Json.Converters;
using MetaBrainz.ListenBrainz.Objects;

namespace MetaBrainz.ListenBrainz.Json.Readers;

internal class ArtistTimeRangeReader : ObjectReader<ArtistTimeRange> {

  public static readonly ArtistTimeRangeReader Instance = new();

  protected override ArtistTimeRange ReadObjectContents(ref Utf8JsonReader reader, JsonSerializerOptions options) {
    int? listenCount = null;
    Guid? mbid = null;
    string? name = null;
    string? timeUnit = null;
    Dictionary<string, object?>? rest = null;
    while (reader.TokenType == JsonTokenType.PropertyName) {
      var prop = reader.GetPropertyName();
      try {
        reader.Read();
        switch (prop) {
          case "artist_mbid":
            mbid = reader.GetOptionalGuid();
            break;
          case "artist_name":
            name = reader.GetString();
            break;
          case "listen_count":
            listenCount = reader.GetInt32();
            break;
          case "time_unit": {
            // LB-1833: the sitewide endpoints return integer values (days-of-month or year, so ushort is enough as a range)
            if (reader.TokenType == JsonTokenType.Number && reader.TryGetUInt16(out var number)) {
              timeUnit = number.ToString(CultureInfo.InvariantCulture);
            }
            else {
              timeUnit = reader.GetString();
            }
            break;
          }
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
    return new ArtistTimeRange {
      Id = mbid,
      ListenCount = listenCount ?? throw new JsonException("Expected listen count not found or null."),
      Name = name ?? throw new JsonException("Expected artist name not found or null."),
      TimeUnit = timeUnit ?? throw new JsonException("Expected time unit not found or null."),
      UnhandledProperties = rest,
    };
  }

}
