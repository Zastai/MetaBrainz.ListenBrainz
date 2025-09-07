using System.Collections.Generic;

using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

internal sealed class SiteReleaseStatistics : Statistics, ISiteReleaseStatistics {

  public IReadOnlyList<IReleaseInfo>? Releases { get; init; }

  public int? Offset { get; init; }

  public int? TotalCount { get; init; }

}
