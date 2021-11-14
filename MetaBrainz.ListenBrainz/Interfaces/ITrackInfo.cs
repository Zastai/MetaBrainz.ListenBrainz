using JetBrains.Annotations;

using MetaBrainz.Common.Json;

namespace MetaBrainz.ListenBrainz.Interfaces;

/// <summary>Information about a listened track.</summary>
[PublicAPI]
public interface ITrackInfo : IJsonBasedObject {

  /// <summary>Additional information about the track.</summary>
  IAdditionalInfo AdditionalInfo { get; }

  /// <summary>The name of the track's artist.</summary>
  string Artist { get; }

  /// <summary>
  /// Mappings to MusicBrainz IDs for this track, as determined by ListenBrainz.<br/>
  /// There are similar fields in <see cref="AdditionalInfo"/>, but those are values supplied at submission time by the client.
  /// </summary>
  IMusicBrainzIdMappings? MusicBrainzIdMappings { get; }

  /// <summary>The name of the track.</summary>
  string Name { get; }

  /// <summary>The name of the release the track was taken from (if any).</summary>
  string? Release { get; }

}
