using System.Collections.Generic;

using JetBrains.Annotations;

using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

/// <inheritdoc cref="ISubmittedTrackInfo"/>
[PublicAPI]
public class SubmittedTrackInfo : ISubmittedTrackInfo {

  /// <summary>Creates new track information.</summary>
  /// <param name="name">The track's name.</param>
  /// <param name="artist">The track's artist.</param>
  /// <param name="release">The track's release.</param>
  public SubmittedTrackInfo(string name, string artist, string? release = null) {
    this.Artist = artist;
    this.Name = name;
    this.Release = release;
  }

  /// <inheritdoc cref="ISubmittedTrackInfo.AdditionalInfo"/>
  public Dictionary<string, object?>? AdditionalInfo { get; set; }

  IReadOnlyDictionary<string, object?>? ISubmittedTrackInfo.AdditionalInfo => this.AdditionalInfo;

  /// <inheritdoc/>
  public string Artist { get; set; }

  /// <inheritdoc/>
  public string Name { get; set; }

  /// <inheritdoc/>
  public string? Release { get; set; }

}
