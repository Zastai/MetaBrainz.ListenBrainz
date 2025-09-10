using System.Collections.Generic;

using MetaBrainz.Common.Json;
using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

internal sealed class ArtistActivity : JsonBasedObject, IArtistActivity {

  public required IReadOnlyList<IArtistActivityInfo> Artists { get; init; }

}
