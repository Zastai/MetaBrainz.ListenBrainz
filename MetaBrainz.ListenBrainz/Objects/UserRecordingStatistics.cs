using System;
using System.Collections.Generic;

using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

internal sealed class UserRecordingStatistics : UserStatistics, IUserRecordingStatistics {

  public UserRecordingStatistics(DateTimeOffset lastUpdated, StatisticsRange range, string user)
  : base(lastUpdated, range, user)
  { }

  public IReadOnlyList<IRecordingInfo>? Recordings { get; init; }

  public int? Offset { get; init; }

  public int? TotalCount { get; init; }

}
