using System;
using System.Collections.Generic;

using MetaBrainz.Common.Json;
using MetaBrainz.ListenBrainz.Interfaces;
using MetaBrainz.ListenBrainz.Interfaces.JSPF;

namespace MetaBrainz.ListenBrainz.Objects.JSPF;

internal sealed class MusicBrainzTrack : JsonBasedObject, IMusicBrainzTrack {

  public DateTimeOffset? Added { get; init; }

  public string? AddedBy { get; init; }

  public IReadOnlyDictionary<string, object?>? AdditionalMetadata { get; init; }

  public IReadOnlyList<Uri>? ArtistIds { get; init; }

  public long? CoverArtId { get; init; }

  public Guid? CoverArtReleaseId { get; init; }

  public IReadOnlyList<IArtistCredit>? Credits { get; init; }

  public Uri? ReleaseId { get; init; }

}
