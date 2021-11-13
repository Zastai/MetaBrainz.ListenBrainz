using System;
using System.Collections.Generic;

using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

internal sealed class UserRecordingStatistics : UserStatistics, IUserRecordingStatistics {

  public UserRecordingStatistics(DateTimeOffset lastUpdated, StatisticsRange range, string user)
  : base(lastUpdated, range, user)
  { }

  public IReadOnlyList<IRecordingInfo>? Recordings { get; set; }

  public int? Offset { get; set; }

  public int? TotalCount { get; set; }

}
