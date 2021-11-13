using JetBrains.Annotations;

namespace MetaBrainz.ListenBrainz.Interfaces;

/// <summary>Information about listens for artists of a particular country.</summary>
[PublicAPI]
public interface IArtistCountryInfo {

  /// <summary>The number of (distinct) artists from this country that were listened to.</summary>
  int ArtistCount { get; }

  /// <summary>The 3-letter country code (ISO 3166-1 alpha-3).</summary>
  string Country { get; }

  /// <summary>The number of listens for artists from this country.</summary>
  int ListenCount { get; }

}
