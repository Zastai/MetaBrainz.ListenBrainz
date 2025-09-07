using JetBrains.Annotations;

namespace MetaBrainz.ListenBrainz.Interfaces;

/// <summary>Statistics about how many times particular artists were listened to.</summary>
[PublicAPI]
public interface ISiteArtistStatistics : IArtistStatistics, IStatistics;
