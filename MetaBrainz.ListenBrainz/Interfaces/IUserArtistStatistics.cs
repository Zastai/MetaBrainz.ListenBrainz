using JetBrains.Annotations;

namespace MetaBrainz.ListenBrainz.Interfaces;

/// <summary>A user's most-listened artists.</summary>
[PublicAPI]
public interface IUserArtistStatistics : IArtistStatistics, IUserStatistics;
