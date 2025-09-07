using JetBrains.Annotations;

namespace MetaBrainz.ListenBrainz.Interfaces;

/// <summary>The most-listened release groups across all of ListenBrainz.</summary>
[PublicAPI]
public interface ISiteReleaseGroupStatistics : IReleaseGroupStatistics, IStatistics;
