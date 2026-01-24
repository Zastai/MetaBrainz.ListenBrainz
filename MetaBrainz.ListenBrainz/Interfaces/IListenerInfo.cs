using System.Collections.Generic;

namespace MetaBrainz.ListenBrainz.Interfaces;

/// <summary>Information about the listeners for an item.</summary>
public interface IListenerInfo {

  /// <summary>The top listeners for this item.</summary>
  IReadOnlyList<ITopListener> TopListeners { get; }

  /// <summary>The total number of listeners for this item.</summary>
  int TotalListeners { get; }

  /// <summary>The total number of listens for this item.</summary>
  int TotalListens { get; }

}
