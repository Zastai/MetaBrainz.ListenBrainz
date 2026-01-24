using System.Collections.Generic;

using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

internal sealed class ArtistActivity : Statistics, IArtistActivity {

  public required IReadOnlyList<IArtistActivityInfo> Artists { get; init; }

}
