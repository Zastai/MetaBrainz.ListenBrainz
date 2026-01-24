using System.Collections.Generic;

namespace MetaBrainz.ListenBrainz.Interfaces;

/// <summary>Artist-oriented listening statistics.</summary>
public interface IArtistActivity : IStatistics {

  /// <summary>Listening statistics for a number of artists.</summary>
  IReadOnlyList<IArtistActivityInfo> Artists { get; }

}
