using System;

namespace MetaBrainz.ListenBrainz.Interfaces;

/// <summary>Information about an artist's top listeners.</summary>
public interface IArtistListeners : IListenerInfo, IStatistics {

  /// <summary>The MusicBrainz ID for the artist.</summary>
  Guid Id { get; }

  /// <summary>The artist's name.</summary>
  string Name { get; }

}
