using System;
using System.Collections.Generic;
using System.Text.Json;

using MetaBrainz.Common.Json;
using MetaBrainz.Common.Json.Converters;
using MetaBrainz.ListenBrainz.Interfaces;
using MetaBrainz.ListenBrainz.Objects;

namespace MetaBrainz.ListenBrainz.Json.Readers;

internal sealed class DailyActivityReader : ObjectReader<DailyActivity> {

  public static readonly DailyActivityReader Instance = new();

  protected override DailyActivity ReadObjectContents(ref Utf8JsonReader reader, JsonSerializerOptions options) {
    IReadOnlyList<IHourlyActivity>? monday = null;
    IReadOnlyList<IHourlyActivity>? tuesday = null;
    IReadOnlyList<IHourlyActivity>? wednesday = null;
    IReadOnlyList<IHourlyActivity>? thursday = null;
    IReadOnlyList<IHourlyActivity>? friday = null;
    IReadOnlyList<IHourlyActivity>? saturday = null;
    IReadOnlyList<IHourlyActivity>? sunday = null;
    Dictionary<string, object?>? rest = null;
    while (reader.TokenType == JsonTokenType.PropertyName) {
      var prop = reader.GetPropertyName();
      try {
        reader.Read();
        switch (prop) {
          case "Monday":
            monday = reader.ReadList(HourlyActivityReader.Instance, options);
            break;
          case "Tuesday":
            tuesday = reader.ReadList(HourlyActivityReader.Instance, options);
            break;
          case "Wednesday":
            wednesday = reader.ReadList(HourlyActivityReader.Instance, options);
            break;
          case "Thursday":
            thursday = reader.ReadList(HourlyActivityReader.Instance, options);
            break;
          case "Friday":
            friday = reader.ReadList(HourlyActivityReader.Instance, options);
            break;
          case "Saturday":
            saturday = reader.ReadList(HourlyActivityReader.Instance, options);
            break;
          case "Sunday":
            sunday = reader.ReadList(HourlyActivityReader.Instance, options);
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
    return new DailyActivity {
      Monday = monday,
      Tuesday = tuesday,
      Wednesday = wednesday,
      Thursday = thursday,
      Friday = friday,
      Saturday = saturday,
      Sunday = sunday,
      UnhandledProperties = rest
    };
  }

}
