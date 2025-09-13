using System.Collections.Generic;

namespace MetaBrainz.ListenBrainz.Interfaces;

/// <summary>Listening information by release year.</summary>
public interface IEraActivity : IStatistics {

  /// <summary>Listening information grouped by release year.</summary>
  IReadOnlyList<IYearlyActivity>? Activity { get; }

}
