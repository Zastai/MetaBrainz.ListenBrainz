using JetBrains.Annotations;

namespace MetaBrainz.ListenBrainz.Interfaces;

/// <summary>Information about the number of artists the user has listened to, grouped by their country.</summary>
[PublicAPI]
public interface IUserArtistMap : IArtistMap, IUserStatistics;
