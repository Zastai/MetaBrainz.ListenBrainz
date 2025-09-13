using System.Collections.Generic;

using MetaBrainz.Common.Json;

namespace MetaBrainz.ListenBrainz.Interfaces;

/// <summary>Listening information by genre and hour of day.</summary>
public interface IGenreActivity : IJsonBasedObject {

  /// <summary>Listening information grouped by genre and hour of day.</summary>
  IReadOnlyList<IGenreActivityDetails>? Activity { get; }

}
