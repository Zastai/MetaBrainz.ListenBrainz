using System;
using System.Collections.Generic;

using MetaBrainz.Common.Json;
using MetaBrainz.ListenBrainz.Interfaces.JSPF;

namespace MetaBrainz.ListenBrainz.Objects.JSPF;

internal sealed class MusicBrainzPlaylist : JsonBasedObject, IMusicBrainzPlaylist {

  public IReadOnlyDictionary<string, object?>? AdditionalMetadata { get; init; }

  public IReadOnlyList<string>? Collaborators { get; init; }

  public Uri? CopiedFrom { get; init; }

  public bool? CopiedFromDeleted { get; init; }

  public string? CreatedFor { get; init; }

  public string? Creator { get; init; }

  public DateTimeOffset? LastModified { get; init; }

  public bool? Public { get; init; }

}
