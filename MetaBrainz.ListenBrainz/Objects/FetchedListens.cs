using System;
using System.Collections.Generic;

using JetBrains.Annotations;

using MetaBrainz.Common.Json;
using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
internal sealed class FetchedListens : JsonBasedObject, IFetchedListens {

  public FetchedListens(IReadOnlyList<IListen> listens, long ts, string user) {
    this.Listens = listens;
    this.Timestamp = DateTimeOffset.FromUnixTimeSeconds(ts);
    this.UnixTimestamp = ts;
    this.User = user;
  }

  public IReadOnlyList<IListen> Listens { get; }

  public DateTimeOffset Timestamp { get; }

  public long UnixTimestamp { get; }

  public string User { get; }

}
