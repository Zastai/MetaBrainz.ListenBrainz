using System.Collections.Generic;

using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

internal sealed class UserReleaseGroupStatistics : UserStatistics, IUserReleaseGroupStatistics {

  public IReadOnlyList<IReleaseGroupInfo>? ReleaseGroups { get; init; }

  public int? Offset { get; init; }

  public int? TotalCount { get; init; }

}
