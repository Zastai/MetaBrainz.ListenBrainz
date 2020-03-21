using System;

using JetBrains.Annotations;

namespace MetaBrainz.ListenBrainz {

  /// <summary>Utility class for working with Unix time values (seconds since 1970-01-01T00:00:00).</summary>
  [PublicAPI]
  public static class UnixTime {

    /// <summary>The epoch for Unix time values (1970-01-01T00:00:00).</summary>
    public static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    /// <summary>Computes the Unix time value corresponding to the specified date/time.</summary>
    /// <param name="value">The date/time to convert to a Unix time value.</param>
    /// <returns>The corresponding Unix time value.</returns>
    public static long Convert(DateTime value) => (long) (value - UnixTime.Epoch).TotalSeconds;

    /// <summary>Computes the date/time corresponding to the specified Unix time value.</summary>
    /// <param name="value">The Unix time value to convert to a date/time.</param>
    /// <returns>The corresponding date/time.</returns>
    public static DateTime Convert(long value) => UnixTime.Epoch.AddSeconds(value);

  }

}
