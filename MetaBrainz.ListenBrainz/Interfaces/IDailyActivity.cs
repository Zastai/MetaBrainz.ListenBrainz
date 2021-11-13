using System.Collections.Generic;

using JetBrains.Annotations;

using MetaBrainz.Common.Json;

namespace MetaBrainz.ListenBrainz.Interfaces;

/// <summary>Information about how a listens are spread across the days of the week.</summary>
[PublicAPI]
public interface IDailyActivity : IJsonBasedObject {

  /// <summary>An overview of how many listens were recorded during a Monday.</summary>
  IReadOnlyList<IHourlyActivity>? Monday { get; }

  /// <summary>An overview of how many listens were recorded during a Tuesday.</summary>
  IReadOnlyList<IHourlyActivity>? Tuesday { get; }

  /// <summary>An overview of how many listens were recorded during a Wednesday.</summary>
  IReadOnlyList<IHourlyActivity>? Wednesday { get; }

  /// <summary>An overview of how many listens were recorded during a Thursday.</summary>
  IReadOnlyList<IHourlyActivity>? Thursday { get; }

  /// <summary>An overview of how many listens were recorded during a Friday.</summary>
  IReadOnlyList<IHourlyActivity>? Friday { get; }

  /// <summary>An overview of how many listens were recorded during a Saturday.</summary>
  IReadOnlyList<IHourlyActivity>? Saturday { get; }

  /// <summary>An overview of how many listens were recorded during a Sunday.</summary>
  IReadOnlyList<IHourlyActivity>? Sunday { get; }

}
