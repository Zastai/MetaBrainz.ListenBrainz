using System;
using System.Collections.Generic;

using MetaBrainz.Common.Json;
using MetaBrainz.ListenBrainz.Interfaces.JSPF;

namespace MetaBrainz.ListenBrainz.Objects.JSPF;

internal sealed class Playlist : JsonBasedObject, IPlaylist {

  public string? Annotation { get; init; }

  public IReadOnlyList<INamedUri>? Attribution { get; init; }

  public string? Creator { get; init; }

  public DateTimeOffset? Date { get; init; }

  public IReadOnlyDictionary<Uri, IReadOnlyList<object?>?>? Extensions { get; init; }

  public Uri? Identifier { get; init; }

  public Uri? Image { get; init; }

  public Uri? Info { get; init; }

  public Uri? License { get; init; }

  public IReadOnlyList<ILink>? Links { get; init; }

  public Uri? Location { get; init; }

  public IReadOnlyList<IMeta>? Metadata { get; init; }

  public string? Title { get; init; }

  public required IReadOnlyList<ITrack> Tracks { get; init; }

}
