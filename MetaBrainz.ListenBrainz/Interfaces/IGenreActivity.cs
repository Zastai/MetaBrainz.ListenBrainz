using System.Collections.Generic;

namespace MetaBrainz.ListenBrainz.Interfaces;

/// <summary>Listening information by genre and hour of day.</summary>
public interface IGenreActivity : IStatistics {

  /// <summary>Listening information grouped by genre and hour of day.</summary>
  IReadOnlyList<IGenreActivityDetails> Activity { get; }

}
