using System.Collections.Generic;

namespace MetaBrainz.ListenBrainz.Interfaces {

  /// <summary>Metadata about a listened track.</summary>
  public interface ITrackMetaData {

    /// <summary>The name of the track's artist.</summary>
    string Artist { get; }

    /// <summary>Additional information about the track.</summary>
    IReadOnlyDictionary<string, object> Info { get; }

    /// <summary>The name of the track.</summary>
    string Name { get; }

    /// <summary>The name of the release the track was taken from (if any).</summary>
    string Release { get; }

  }

}
