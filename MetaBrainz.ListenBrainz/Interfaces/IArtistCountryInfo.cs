using System.Collections.Generic;

using JetBrains.Annotations;

namespace MetaBrainz.ListenBrainz.Interfaces;

/// <summary>Information about listens for artists from a particular country.</summary>
[PublicAPI]
public interface IArtistCountryInfo {

  /// <summary>The number of (distinct) artists from this country that were listened to.</summary>
  int ArtistCount { get; }

  /// <summary>Information about the artists from this country that were listened to.</summary>
  IReadOnlyList<IArtistInfo>? Artists { get; }

  /// <summary>The 3-letter country code (ISO 3166-1 alpha-3).</summary>
  string Country { get; }

  /// <summary>The number of listens for artists from this country.</summary>
  int ListenCount { get; }

}
