using System.Collections.Generic;

using MetaBrainz.Common.Json;
using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

internal sealed class DailyActivity : JsonBasedObject, IDailyActivity {

  public IReadOnlyList<IHourlyActivity>? Monday { get; init; }

  public IReadOnlyList<IHourlyActivity>? Tuesday { get; init; }

  public IReadOnlyList<IHourlyActivity>? Wednesday { get; init; }

  public IReadOnlyList<IHourlyActivity>? Thursday { get; init; }

  public IReadOnlyList<IHourlyActivity>? Friday { get; init; }

  public IReadOnlyList<IHourlyActivity>? Saturday { get; init; }

  public IReadOnlyList<IHourlyActivity>? Sunday { get; init; }

}
