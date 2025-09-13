using MetaBrainz.Common.Json;

namespace MetaBrainz.ListenBrainz;

/// <summary>The range covered by a set of statistics.</summary>
public enum StatisticsRange {

  /// <summary>The statistics cover all available data.</summary>
  AllTime,

  /// <summary>The statistics cover half a year of data.</summary>
  HalfYearly,

  /// <summary>The statistics cover a whole month of data.</summary>
  Month,

  /// <summary>The statistics cover a quarter of data.</summary>
  Quarter,

  /// <summary>The statistics cover the current month's data.</summary>
  ThisMonth,

  /// <summary>The statistics cover the current week's data.</summary>
  ThisWeek,

  /// <summary>The statistics cover the current year's data.</summary>
  ThisYear,

  /// <summary>The statistics cover a week of data.</summary>
  Week,

  /// <summary>The statistics cover a year of data.</summary>
  Year,

  /// <summary>
  /// The range was specified as an unknown value; check the object's <see cref="IJsonBasedObject.UnhandledProperties"/> for the
  /// specific value used.
  /// </summary>
  Unknown,

}
