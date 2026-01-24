using System.Collections.Generic;

using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

internal sealed class ArtistEvolutionActivity : Statistics, IArtistEvolutionActivity {

  public required IReadOnlyList<IArtistTimeRange> Activity { get; init; }

}
