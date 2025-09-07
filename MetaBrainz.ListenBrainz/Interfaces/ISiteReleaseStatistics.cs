using JetBrains.Annotations;

namespace MetaBrainz.ListenBrainz.Interfaces;

/// <summary>The most-listened releases across all of ListenBrainz.</summary>
[PublicAPI]
public interface ISiteReleaseStatistics : IReleaseStatistics, IStatistics;
