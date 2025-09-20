using System;

namespace MetaBrainz.ListenBrainz.Interfaces;

/// <summary>Artist listening information over a given period of time.</summary>
public interface IArtistTimeRange {

  /// <summary>The MusicBrainz ID for the artist.</summary>
  Guid? Id { get; }

  /// <summary>The number of times the artist's tracks were listened to.</summary>
  int ListenCount { get; }

  /// <summary>The artist's name.</summary>
  string Name { get; }

  /// <summary>The time unit describing the period for the listen count.</summary>
  /// <remarks>
  /// This depends on the requested range for the statistics.
  /// <list type="table">
  ///   <listheader>
  ///     <term>Statistics Range</term>
  ///     <description>Time Unit</description>
  ///   </listheader>
  ///   <item>
  ///     <term><see cref="StatisticsRange.AllTime"/>, <see cref="StatisticsRange.Quarter"/></term>
  ///     <description>The (Gregorian) calendar year (e.g. "2019" or "2025").</description>
  ///   </item>
  ///   <item>
  ///     <term>
  ///       <see cref="StatisticsRange.HalfYearly"/>, <see cref="StatisticsRange.ThisYear"/>, <see cref="StatisticsRange.Year"/>
  ///     </term>
  ///     <description>The (Gregorian, English) month name (e.g. "January" or "August").</description>
  ///   </item>
  ///   <item>
  ///     <term><see cref="StatisticsRange.Month"/>, <see cref="StatisticsRange.ThisMonth"/></term>
  ///     <description>The day of the month (e.g. "7" or "13").</description>
  ///   </item>
  ///   <item>
  ///     <term><see cref="StatisticsRange.Week"/>, <see cref="StatisticsRange.ThisWeek"/></term>
  ///     <description>The (English) weekday name (e.g. "Sunday" or "Wednesday").</description>
  ///   </item>
  /// </list>
  /// </remarks>
  string TimeUnit { get; }

}
