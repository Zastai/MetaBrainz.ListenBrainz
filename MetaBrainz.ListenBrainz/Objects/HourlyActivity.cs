using MetaBrainz.Common.Json;
using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

internal sealed class HourlyActivity : JsonBasedObject, IHourlyActivity {

  public HourlyActivity(int hour, int listenCount) {
    this.Hour = hour;
    this.ListenCount = listenCount;
  }

  public int Hour { get; }

  public int ListenCount { get; }

}
