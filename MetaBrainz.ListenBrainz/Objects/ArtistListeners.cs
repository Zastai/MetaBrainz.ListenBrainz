using System;

using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

internal sealed class ArtistListeners : ListenerInfo, IArtistListeners {

  public required Guid Id { get; init; }

  public required string Name { get; init; }

}
