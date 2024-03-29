using JetBrains.Annotations;

using MetaBrainz.Common.Json;
using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
internal sealed class ListenCount : JsonBasedObject, IListenCount {

  public ListenCount(long count) {
    this.Count = count;
  }

  public long Count { get; }

}
