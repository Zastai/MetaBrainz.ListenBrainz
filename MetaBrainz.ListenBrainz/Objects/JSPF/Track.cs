using System;
using System.Collections.Generic;

using MetaBrainz.Common.Json;
using MetaBrainz.ListenBrainz.Interfaces.JSPF;

namespace MetaBrainz.ListenBrainz.Objects.JSPF;

internal sealed class Track : JsonBasedObject, ITrack {

  public string? Album { get; init; }

  public string? Annotation { get; init; }

  public string? Creator { get; init; }

  public TimeSpan? Duration { get; init; }

  public IReadOnlyDictionary<Uri, IReadOnlyList<object?>?>? Extensions { get; init; }

  public IReadOnlyList<Uri>? Identifiers { get; init; }

  public Uri? Image { get; init; }

  public Uri? Info { get; init; }

  public IReadOnlyList<ILink>? Links { get; init; }

  public IReadOnlyList<Uri>? Locations { get; init; }

  public IReadOnlyList<IMeta>? Metadata { get; init; }

  public string? Title { get; init; }

  public uint? TrackNumber { get; init; }

}
