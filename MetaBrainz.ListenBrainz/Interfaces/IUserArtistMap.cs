using System.Collections.Generic;

using JetBrains.Annotations;

namespace MetaBrainz.ListenBrainz.Interfaces;

/// <summary>Information about the number of artists the user has listened to, grouped by their country.</summary>
[PublicAPI]
public interface IUserArtistMap : IUserStatistics {

  /// <summary>The per-country listen information.</summary>
  IReadOnlyList<IArtistCountryInfo>? Countries { get; }

}
