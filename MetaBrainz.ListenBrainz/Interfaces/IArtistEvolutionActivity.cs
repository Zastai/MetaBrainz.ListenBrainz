using System.Collections.Generic;

namespace MetaBrainz.ListenBrainz.Interfaces;

/// <summary>Listening information for artists over time.</summary>
public interface IArtistEvolutionActivity : IStatistics {

  /// <summary>Artist listening information over a given period of time.</summary>
  IReadOnlyList<IArtistTimeRange> Activity { get; }

}
