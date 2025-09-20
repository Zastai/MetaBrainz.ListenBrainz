using System;
using System.Collections.Generic;

using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

internal sealed class ReleaseGroupListeners : ListenerInfo, IReleaseGroupListeners {

  public IReadOnlyList<Guid>? ArtistIds { get; init; }

  public string? ArtistName { get; init; }

  public long? CoverArtId { get; init; }

  public Guid? CoverArtReleaseId { get; init; }

  public required Guid Id { get; init; }

  public required string Name { get; init; }

}
