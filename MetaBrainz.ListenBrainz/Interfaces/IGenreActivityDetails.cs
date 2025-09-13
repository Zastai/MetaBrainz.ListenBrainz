using JetBrains.Annotations;

using MetaBrainz.Common.Json;

namespace MetaBrainz.ListenBrainz.Interfaces;

/// <summary>The number of listens associated with a particular genre at a particular hour of the day.</summary>
[PublicAPI]
public interface IGenreActivityDetails : IHourlyActivity {

  /// <summary>The genre.</summary>
  string Genre { get; }

}
