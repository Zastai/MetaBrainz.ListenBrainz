using System;

using JetBrains.Annotations;

using MetaBrainz.Common.Json;
using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
internal sealed class LatestImport : JsonBasedObject, ILatestImport {

  public DateTimeOffset Timestamp => DateTimeOffset.FromUnixTimeSeconds(this.UnixTimestamp);

  public required long UnixTimestamp { get; init; }

  public required string? User { get; init; }

}
