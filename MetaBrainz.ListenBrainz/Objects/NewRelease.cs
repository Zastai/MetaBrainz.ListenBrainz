using System;
using System.Collections.Generic;

using MetaBrainz.Common.Json;
using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

internal sealed class NewRelease : JsonBasedObject, INewRelease {

  public long? CoverArtId { get; init; }

  public Guid? CoverArtReleaseId { get; init; }

  public string? CreditedArtist { get; init; }

  public IReadOnlyList<Guid>? CreditedArtistIds { get; init; }

  public IReadOnlyList<string>? CreditedArtistNames { get; init; }

  public IReadOnlyList<IArtistCredit>? CreditedArtists { get; init; }

  public string? FirstReleaseDate { get; init; }

  public Guid? ReleaseId { get; init; }

  public Guid? ReleaseGroupId { get; init; }

  public required string Title { get; init; }

  public string? Type { get; init; }

}
