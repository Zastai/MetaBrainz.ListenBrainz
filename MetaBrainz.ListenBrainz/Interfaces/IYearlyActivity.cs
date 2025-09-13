using JetBrains.Annotations;

using MetaBrainz.Common.Json;

namespace MetaBrainz.ListenBrainz.Interfaces;

/// <summary>The number of listens associated with a particular year.</summary>
[PublicAPI]
public interface IYearlyActivity : IJsonBasedObject {

  /// <summary>The number of listens recorded.</summary>
  int ListenCount { get; }

  /// <summary>The year.</summary>
  int Year { get; }

}
