using System;
using System.Collections.Generic;

using MetaBrainz.Common.Json;
using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

internal sealed class RecordingInfo : JsonBasedObject, IRecordingInfo {

  public IReadOnlyList<Guid>? ArtistIds { get; init; }

  public Guid? ArtistMessyId { get; init; }

  public string? ArtistName { get; init; }

  public long? CoverArtId { get; init; }

  public Guid? CoverArtReleaseId { get; init; }

  public IReadOnlyList<IArtistCredit>? Credits { get; init; }

  public required int ListenCount { get; init; }

  public Guid? Id { get; init; }

  public Guid? MessyId { get; init; }

  public required string Name { get; init; }

  public Guid? ReleaseId { get; init; }

  public Guid? ReleaseMessyId { get; init; }

  public string? ReleaseName { get; init; }

}
