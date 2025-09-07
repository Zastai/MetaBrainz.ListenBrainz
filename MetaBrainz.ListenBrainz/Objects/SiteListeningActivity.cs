using System;
using System.Collections.Generic;

using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

internal sealed class SiteListeningActivity(DateTimeOffset lastUpdated, StatisticsRange range)
  : Statistics(lastUpdated, range), ISiteListeningActivity {

  public IReadOnlyList<IListenTimeRange>? Activity { get; init; }

}
