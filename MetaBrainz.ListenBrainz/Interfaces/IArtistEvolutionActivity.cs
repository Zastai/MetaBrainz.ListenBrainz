using System.Collections.Generic;

namespace MetaBrainz.ListenBrainz.Interfaces;

/// <summary>Listening information for artists over time.</summary>
public interface IArtistEvolutionActivity : IStatistics {

  /// <summary>Artist listening information over a given period of time.</summary>
  IReadOnlyList<IArtistTimeRange>? Activity { get; }

  /// <summary>The user for whom the information was computed, or <see langword="null"/> if the information is site-wide.</summary>
  string? User { get; }

}
