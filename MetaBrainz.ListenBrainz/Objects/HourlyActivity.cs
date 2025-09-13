using MetaBrainz.Common.Json;
using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

internal sealed class HourlyActivity : JsonBasedObject, IHourlyActivity {

  public required int Hour { get; init; }

  public required int ListenCount { get; init; }

}
