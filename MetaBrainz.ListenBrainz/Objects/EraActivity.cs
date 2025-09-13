using System.Collections.Generic;

using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

internal sealed class EraActivity : Statistics, IEraActivity {

  public IReadOnlyList<IYearlyActivity>? Activity { get; init; }

}
