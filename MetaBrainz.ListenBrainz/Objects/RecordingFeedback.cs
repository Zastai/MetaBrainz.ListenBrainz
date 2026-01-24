using System;

using MetaBrainz.Common.Json;
using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

internal sealed class RecordingFeedback : JsonBasedObject, IRecordingFeedback {

  public required DateTimeOffset Created { get; init; }

  public required Guid? Id { get; init; }

  public required Guid? MessyId { get; init; }

  public required int Score { get; init; }

  public required object? TrackMetadata { get; init; }

  public required string User { get; init; }

}
