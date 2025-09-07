using System.Collections.Generic;

using JetBrains.Annotations;

namespace MetaBrainz.ListenBrainz.Interfaces;

/// <summary>Information about how many listens have been submitted over a period of time.</summary>
[PublicAPI]
public interface IListeningActivity {

  /// <summary>The submitted listens.</summary>
  IReadOnlyList<IListenTimeRange>? Activity { get; }

}
