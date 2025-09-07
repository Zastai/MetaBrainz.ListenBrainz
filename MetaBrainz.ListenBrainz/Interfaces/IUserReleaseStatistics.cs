using JetBrains.Annotations;

namespace MetaBrainz.ListenBrainz.Interfaces;

/// <summary>A user's most-listened releases.</summary>
[PublicAPI]
public interface IUserReleaseStatistics : IReleaseStatistics, IUserStatistics;
