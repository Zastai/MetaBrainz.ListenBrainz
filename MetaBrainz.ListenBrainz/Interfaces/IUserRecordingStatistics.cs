using JetBrains.Annotations;

namespace MetaBrainz.ListenBrainz.Interfaces;

/// <summary>A user's most-listened recordings ("tracks").</summary>
[PublicAPI]
public interface IUserRecordingStatistics : IRecordingStatistics, IUserStatistics;
