using MetaBrainz.Common.Json;
using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

internal sealed class YearlyActivity : JsonBasedObject, IYearlyActivity {

  public required int ListenCount { get; init; }

  public required int Year { get; init; }

}
