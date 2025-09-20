using System;
using System.Collections.Generic;

using MetaBrainz.Common.Json;
using MetaBrainz.ListenBrainz.Interfaces.JSPF;

namespace MetaBrainz.ListenBrainz.Objects.JSPF;

internal sealed class MusicBrainzRecording : JsonBasedObject, IMusicBrainzRecording {

  public IReadOnlyList<Guid>? ArtistIds { get; init; }

}
