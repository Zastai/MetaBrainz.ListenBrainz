using System.Collections.Generic;

using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

internal sealed class GenreActivity : Statistics, IGenreActivity {

  public required IReadOnlyList<IGenreActivityDetails> Activity { get; init; }

}
