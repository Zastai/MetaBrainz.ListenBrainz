using System;
using System.Collections.Generic;

using JetBrains.Annotations;

using MetaBrainz.Common.Json;
using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
internal sealed class FetchedListens : JsonBasedObject, IFetchedListens {

  public FetchedListens(IReadOnlyList<IListen> listens, long newest, long oldest, string user) {
    this.Listens = listens;
    this.Newest = DateTimeOffset.FromUnixTimeSeconds(newest);
    this.Oldest = DateTimeOffset.FromUnixTimeSeconds(oldest);
    this.User = user;
  }

  public IReadOnlyList<IListen> Listens { get; }

  public DateTimeOffset Newest { get; }

  public DateTimeOffset Oldest { get; }

  public string User { get; }

}
