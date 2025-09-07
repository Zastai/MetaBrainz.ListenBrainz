using System.Collections.Generic;

using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

internal abstract class ListenerInfo : Statistics, IListenerInfo {

  public required IReadOnlyList<ITopListener> TopListeners { get; init; }

  public required int TotalListens { get; init; }

  public required int TotalListeners { get; init; }

}
