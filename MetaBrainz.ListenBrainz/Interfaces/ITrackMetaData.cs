using JetBrains.Annotations;

using MetaBrainz.Common.Json;

namespace MetaBrainz.ListenBrainz.Interfaces {

  /// <summary>Metadata about a listened track.</summary>
  [PublicAPI]
  public interface ITrackMetaData : IJsonBasedObject {

    /// <summary>Additional information about the track.</summary>
    IAdditionalInfo? AdditionalInfo { get; }

    /// <summary>The name of the track's artist.</summary>
    string? Artist { get; }

    /// <summary>The name of the track.</summary>
    string? Name { get; }

    /// <summary>The name of the release the track was taken from (if any).</summary>
    string? Release { get; }

  }

}
