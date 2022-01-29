using System;

using JetBrains.Annotations;

using MetaBrainz.Common;
using MetaBrainz.Common.Json;
using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
internal sealed class LatestImport : JsonBasedObject, ILatestImport {

  public LatestImport(long ts, string user) {
    this.Timestamp = UnixTime.Convert(ts);
    this.UnixTimestamp = ts;
    this.User = user;
  }

  public DateTimeOffset? Timestamp { get; }

  public long? UnixTimestamp { get; }

  public string? User { get; }

}
