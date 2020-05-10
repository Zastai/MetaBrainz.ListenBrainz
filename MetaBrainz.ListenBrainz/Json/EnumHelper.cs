namespace MetaBrainz.ListenBrainz.Json {

  internal static class EnumHelper {

    public static StatisticsRange? ParseStatisticsRange(string text) {
      if (text == null)
        return null;
      return text switch {
        "all_time" => StatisticsRange.AllTime,
        _ => StatisticsRange.Unknown,
      };
    }

  }

}
