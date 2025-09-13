using System.Collections.Generic;

using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

internal sealed class ListeningActivity : Statistics, IListeningActivity {

  public IReadOnlyList<IListenTimeRange>? Activity { get; init; }

}
