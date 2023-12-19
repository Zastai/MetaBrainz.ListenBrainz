using System;
using System.Diagnostics.CodeAnalysis;

namespace MetaBrainz.ListenBrainz.Json;

internal static class EnumHelper {

  [return: NotNullIfNotNull(nameof(text))]
  public static StatisticsRange? ParseStatisticsRange(string? text) {
    if (text is null) {
      return null;
    }
    return text switch {
      "all_time" => StatisticsRange.AllTime,
      "week" => StatisticsRange.Week,
      "month" => StatisticsRange.Month,
      "year" => StatisticsRange.Year,
      _ => StatisticsRange.Unknown,
    };
  }

  public static string ToJson(this StatisticsRange range) => range switch {
    StatisticsRange.AllTime => "all_time",
    StatisticsRange.Week => "week",
    StatisticsRange.Month => "month",
    StatisticsRange.Year => "year",
    _ => throw new ArgumentOutOfRangeException(nameof(range), range, "Invalid statistics range specified.")
  };

}
