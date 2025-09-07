using JetBrains.Annotations;

namespace MetaBrainz.ListenBrainz.Interfaces;

/// <summary>
/// Information about the number of times artists are listened to, grouped by their country, across all of ListenBrainz.
/// </summary>
[PublicAPI]
public interface ISiteArtistMap : IArtistMap, IStatistics;
