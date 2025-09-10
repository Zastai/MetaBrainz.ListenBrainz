using System.Collections.Generic;

using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

internal sealed class SiteListeningActivity : Statistics, ISiteListeningActivity {

  public IReadOnlyList<IListenTimeRange>? Activity { get; init; }

}
