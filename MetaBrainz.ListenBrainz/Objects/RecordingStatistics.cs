using System.Collections.Generic;

using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

internal sealed class RecordingStatistics : Statistics, IRecordingStatistics {

  public IReadOnlyList<IRecordingInfo>? Recordings { get; init; }

  public int? Offset { get; init; }

  public int? TotalCount { get; init; }

}
