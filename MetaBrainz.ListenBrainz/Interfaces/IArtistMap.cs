using System.Collections.Generic;

using JetBrains.Annotations;

namespace MetaBrainz.ListenBrainz.Interfaces;

/// <summary>Information about artist listen counts, grouped by country.</summary>
[PublicAPI]
public interface IArtistMap {

  /// <summary>The per-country listen information.</summary>
  IReadOnlyList<IArtistCountryInfo>? Countries { get; }

}
