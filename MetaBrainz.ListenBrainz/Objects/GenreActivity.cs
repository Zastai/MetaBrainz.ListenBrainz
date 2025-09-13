using System.Collections.Generic;

using MetaBrainz.Common.Json;
using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

internal sealed class GenreActivity : JsonBasedObject, IGenreActivity {

  public IReadOnlyList<IGenreActivityDetails>? Activity { get; init; }

}
