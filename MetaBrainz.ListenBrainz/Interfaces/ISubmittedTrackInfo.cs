using System.Collections.Generic;

using JetBrains.Annotations;

namespace MetaBrainz.ListenBrainz.Interfaces {

  /// <summary>Information about a listened track.</summary>
  [PublicAPI]
  public interface ISubmittedTrackInfo {

    /// <summary>Additional information about the track.</summary>
    IReadOnlyDictionary<string, object?>? AdditionalInfo { get; }

    /// <summary>The name of the track's artist.</summary>
    string Artist { get; }

    /// <summary>The name of the track.</summary>
    string Name { get; }

    /// <summary>The name of the release the track was taken from (if any).</summary>
    string? Release { get; }

  }

}
