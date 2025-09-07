using System;
using System.Collections.Generic;

using JetBrains.Annotations;

using MetaBrainz.Common.Json;
using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
internal sealed class FetchedListens : JsonBasedObject, IFetchedListens {

  public required IReadOnlyList<IListen> Listens { get; init; }

  public required DateTimeOffset Newest { get; init; }

  public required DateTimeOffset Oldest { get; init; }

  public required string User { get; init; }

}
