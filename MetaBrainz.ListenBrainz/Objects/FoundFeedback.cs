using System.Collections.Generic;

using MetaBrainz.Common.Json;
using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

internal sealed class FoundFeedback : JsonBasedObject, IFoundFeedback {

  public required int Count { get; init; }

  public required int Offset { get; init; }

  public required IReadOnlyList<IRecordingFeedback> Feedback { get; init; }

  public required int TotalCount { get; init; }

}
