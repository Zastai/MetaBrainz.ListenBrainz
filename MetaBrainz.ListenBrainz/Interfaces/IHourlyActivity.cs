using JetBrains.Annotations;

using MetaBrainz.Common.Json;

namespace MetaBrainz.ListenBrainz.Interfaces {

  /// <summary>The number of listens that were recorded during a particular hour of the day.</summary>
  [PublicAPI]
  public interface IHourlyActivity : IJsonBasedObject {

    /// <summary>The hour (0-23).</summary>
    int Hour { get; }

    /// <summary>The number of listens recorded this hour.</summary>
    int ListenCount { get; }

  }

}
