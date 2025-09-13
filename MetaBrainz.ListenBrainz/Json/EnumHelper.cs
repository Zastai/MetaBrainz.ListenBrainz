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
      "half_yearly" => StatisticsRange.HalfYearly,
      "month" => StatisticsRange.Month,
      "quarter" => StatisticsRange.Quarter,
      "this_month" => StatisticsRange.ThisMonth,
      "this_week" => StatisticsRange.ThisWeek,
      "this_year" => StatisticsRange.ThisYear,
      "week" => StatisticsRange.Week,
      "year" => StatisticsRange.Year,
      _ => StatisticsRange.Unknown,
    };
  }

  public static string ToJson(this StatisticsRange range) => range switch {
    StatisticsRange.AllTime => "all_time",
    StatisticsRange.HalfYearly => "half_yearly",
    StatisticsRange.Month => "month",
    StatisticsRange.Quarter => "quarter",
    StatisticsRange.ThisMonth => "this_month",
    StatisticsRange.ThisWeek => "this_week",
    StatisticsRange.ThisYear => "this_year",
    StatisticsRange.Week => "week",
    StatisticsRange.Year => "year",
    _ => throw new ArgumentOutOfRangeException(nameof(range), range, "Invalid statistics range specified.")
  };

}
