using System;
using System.Text.Json;
using System.Text.Json.Serialization;

using JetBrains.Annotations;

namespace MetaBrainz.ListenBrainz {

  /// <summary>Utility class for working with Unix time values (seconds since 1970-01-01T00:00:00).</summary>
  [PublicAPI]
  public static class UnixTime {

    /// <summary>The epoch for Unix time values (1970-01-01T00:00:00Z).</summary>
#if NETSTD_GE_2_1 || NETCORE_GE_2_1
    public static readonly DateTime Epoch = DateTime.UnixEpoch;
#else
    public static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
#endif

    /// <summary>Computes the Unix time value corresponding to the specified date/time.</summary>
    /// <param name="value">The date/time to convert to a Unix time value.</param>
    /// <returns>The corresponding Unix time value.</returns>
    public static long Convert(DateTime value) => (long) (value - UnixTime.Epoch).TotalSeconds;

    /// <summary>Computes the Unix time value corresponding to the specified date/time.</summary>
    /// <param name="value">The date/time to convert to a Unix time value.</param>
    /// <returns>The corresponding Unix time value.</returns>
    public static long? Convert(DateTime? value) => value.HasValue ? (long?) (value.Value - UnixTime.Epoch).TotalSeconds : null;

    /// <summary>Computes the date/time corresponding to the specified Unix time value.</summary>
    /// <param name="value">The Unix time value to convert to a date/time.</param>
    /// <returns>The corresponding date/time.</returns>
    public static DateTime Convert(long value) => UnixTime.Epoch.AddSeconds(value);

    /// <summary>Computes the date/time corresponding to the specified Unix time value.</summary>
    /// <param name="value">The Unix time value to convert to a date/time.</param>
    /// <returns>The corresponding date/time.</returns>
    public static DateTime? Convert(long? value) => value.HasValue ? (DateTime?) UnixTime.Epoch.AddSeconds(value.Value) : null;

    /// <summary>A JSON converter for serializing Unix time values to/from .NET <see cref="DateTime"/> values.
    /// </summary>
    public sealed class JsonConverter : JsonConverter<DateTime?> {

      /// <inheritdoc />
      public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        if (reader.TokenType == JsonTokenType.Null)
          return null;
        if (reader.TokenType == JsonTokenType.Number && reader.TryGetInt64(out var unixTime))
          return UnixTime.Convert(unixTime);
        throw new JsonException("The value for a Unix time field must be a valid 64-bit integer.");
      }

      /// <inheritdoc />
      public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options) {
        if (value.HasValue)
          writer.WriteNumberValue(UnixTime.Convert(value.Value));
        else
          writer.WriteNullValue();
      }

    }

  }

}
