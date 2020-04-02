using System;
using System.Text.Json;
using System.Text.Json.Serialization;

using JetBrains.Annotations;

namespace MetaBrainz.ListenBrainz {

  /// <summary>Utility class for working with Unix time values (seconds since 1970-01-01T00:00:00).</summary>
  [PublicAPI]
  public static class UnixTime {

#if NETSTD_GE_2_1 || NETCORE_GE_2_1 // These have Unix Time support built in

    /// <summary>The epoch for Unix time values (1970-01-01T00:00:00Z).</summary>
    public static readonly DateTimeOffset Epoch = DateTimeOffset.UnixEpoch;

    /// <summary>Computes the Unix time value corresponding to the specified date/time.</summary>
    /// <param name="value">The date/time to convert to a Unix time value.</param>
    /// <returns>The corresponding Unix time value.</returns>
    public static long Convert(DateTimeOffset value) => value.ToUnixTimeSeconds();

    /// <summary>Computes the Unix time value corresponding to the specified date/time.</summary>
    /// <param name="value">The date/time to convert to a Unix time value.</param>
    /// <returns>The corresponding Unix time value.</returns>
    public static long? Convert(DateTimeOffset? value) => value?.ToUnixTimeSeconds();

    /// <summary>Computes the date/time corresponding to the specified Unix time value.</summary>
    /// <param name="value">The Unix time value to convert to a date/time.</param>
    /// <returns>The corresponding date/time.</returns>
    public static DateTimeOffset Convert(long value) => DateTimeOffset.FromUnixTimeSeconds(value);

    /// <summary>Computes the date/time corresponding to the specified Unix time value.</summary>
    /// <param name="value">The Unix time value to convert to a date/time.</param>
    /// <returns>The corresponding date/time.</returns>
    public static DateTimeOffset? Convert(long? value)
      => value.HasValue ? (DateTimeOffset?) DateTimeOffset.FromUnixTimeSeconds(value.Value) : null;

#else

    /// <summary>The epoch for Unix time values (1970-01-01T00:00:00Z).</summary>
    public static readonly DateTimeOffset Epoch = new DateTimeOffset(621355968000000000L, TimeSpan.Zero);

    /// <summary>Computes the Unix time value corresponding to the specified date/time.</summary>
    /// <param name="value">The date/time to convert to a Unix time value.</param>
    /// <returns>The corresponding Unix time value.</returns>
    public static long Convert(DateTimeOffset value) => (long) (value - UnixTime.Epoch).TotalSeconds;

    /// <summary>Computes the Unix time value corresponding to the specified date/time.</summary>
    /// <param name="value">The date/time to convert to a Unix time value.</param>
    /// <returns>The corresponding Unix time value.</returns>
    public static long? Convert(DateTimeOffset? value)
      => value.HasValue ? (long?) (value.Value - UnixTime.Epoch).TotalSeconds : null;

    /// <summary>Computes the date/time corresponding to the specified Unix time value.</summary>
    /// <param name="value">The Unix time value to convert to a date/time.</param>
    /// <returns>The corresponding date/time.</returns>
    public static DateTimeOffset Convert(long value) => UnixTime.Epoch.AddSeconds(value);

    /// <summary>Computes the date/time corresponding to the specified Unix time value.</summary>
    /// <param name="value">The Unix time value to convert to a date/time.</param>
    /// <returns>The corresponding date/time.</returns>
    public static DateTimeOffset? Convert(long? value)
      => value.HasValue ? (DateTimeOffset?) UnixTime.Epoch.AddSeconds(value.Value) : null;

#endif

    /// <summary>
    /// A JSON converter for serializing Unix time values to/from .NET <see cref="DateTimeOffset"/> values (always UTC).
    /// </summary>
    public sealed class JsonConverter : JsonConverter<DateTimeOffset?> {

      /// <inheritdoc />
      public override DateTimeOffset? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        if (reader.TokenType == JsonTokenType.Null)
          return null;
        if (reader.TokenType == JsonTokenType.Number && reader.TryGetInt64(out var unixTime))
          return UnixTime.Convert(unixTime);
        throw new JsonException("The value for a Unix time field must be a valid 64-bit integer.");
      }

      /// <inheritdoc />
      public override void Write(Utf8JsonWriter writer, DateTimeOffset? value, JsonSerializerOptions options) {
        if (value.HasValue)
          writer.WriteNumberValue(UnixTime.Convert(value.Value));
        else
          writer.WriteNullValue();
      }

    }

  }

}
