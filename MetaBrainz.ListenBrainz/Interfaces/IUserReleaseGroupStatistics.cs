using JetBrains.Annotations;

namespace MetaBrainz.ListenBrainz.Interfaces;

/// <summary>A user's most-listened release groups.</summary>
[PublicAPI]
public interface IUserReleaseGroupStatistics : IReleaseGroupStatistics, IUserStatistics;
