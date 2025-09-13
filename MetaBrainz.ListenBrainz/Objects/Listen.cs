using System;

using JetBrains.Annotations;

using MetaBrainz.Common.Json;
using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
internal sealed class Listen : JsonBasedObject, IListen {

  public required DateTimeOffset InsertedAt { get; init; }

  public required DateTimeOffset ListenedAt { get; init; }

  public required Guid MessyRecordingId { get; init; }

  public required ITrackInfo Track { get; init; }

  public required string User { get; init; }

}
