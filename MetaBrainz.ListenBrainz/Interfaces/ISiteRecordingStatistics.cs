using JetBrains.Annotations;

namespace MetaBrainz.ListenBrainz.Interfaces;

/// <summary>The most-listened recordings ("tracks") across all of ListenBrainz.</summary>
[PublicAPI]
public interface ISiteRecordingStatistics : IRecordingStatistics, IStatistics;
