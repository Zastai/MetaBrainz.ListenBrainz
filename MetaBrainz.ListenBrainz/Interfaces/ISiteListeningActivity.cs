using JetBrains.Annotations;

namespace MetaBrainz.ListenBrainz.Interfaces;

/// <summary>Information about how many listens have been submitted over a period of time across all of ListenBrainz.</summary>
[PublicAPI]
public interface ISiteListeningActivity : IListeningActivity, IStatistics;
