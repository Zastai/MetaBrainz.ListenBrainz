using System.Collections.Generic;

using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

internal sealed class ReleaseGroupStatistics : Statistics, IReleaseGroupStatistics {

  public int? Offset { get; init; }

  public IReadOnlyList<IReleaseGroupInfo>? ReleaseGroups { get; init; }

  public int? TotalCount { get; init; }

}
