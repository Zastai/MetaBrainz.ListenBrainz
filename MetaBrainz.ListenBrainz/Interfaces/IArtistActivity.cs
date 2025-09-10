using System.Collections.Generic;

using MetaBrainz.Common.Json;

namespace MetaBrainz.ListenBrainz.Interfaces;

/// <summary>Artist-oriented listening statistics.</summary>
public interface IArtistActivity : IJsonBasedObject {

  /// <summary>Listening statistics for a number of artists.</summary>
  IReadOnlyList<IArtistActivityInfo> Artists { get; }

}
