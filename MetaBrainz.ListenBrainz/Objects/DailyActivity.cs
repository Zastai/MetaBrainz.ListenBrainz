using System.Collections.Generic;

using MetaBrainz.Common.Json;
using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects {

  internal sealed class DailyActivity : JsonBasedObject, IDailyActivity {

    public IReadOnlyList<IHourlyActivity>? Monday { get; set; }

    public IReadOnlyList<IHourlyActivity>? Tuesday { get; set; }

    public IReadOnlyList<IHourlyActivity>? Wednesday { get; set; }

    public IReadOnlyList<IHourlyActivity>? Thursday { get; set; }

    public IReadOnlyList<IHourlyActivity>? Friday { get; set; }

    public IReadOnlyList<IHourlyActivity>? Saturday { get; set; }

    public IReadOnlyList<IHourlyActivity>? Sunday { get; set; }

  }

}
