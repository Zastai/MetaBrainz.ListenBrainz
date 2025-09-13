using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using JetBrains.Annotations;

using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

/// <inheritdoc cref="ISubmittedTrackInfo"/>
[PublicAPI]
public class SubmittedTrackInfo : ISubmittedTrackInfo {

  /// <summary>Creates new track information.</summary>
  /// <remarks>
  /// This constructor is only here to enable a plain object initializer and will be removed once the other constructor is.
  /// </remarks>
  public SubmittedTrackInfo() { }

  /// <summary>Creates new track information.</summary>
  /// <param name="name">The track's name.</param>
  /// <param name="artist">The track's artist.</param>
  /// <param name="release">The track's release.</param>
  [Obsolete("Use an object initializer to set the properties.")]
  [SetsRequiredMembers]
  public SubmittedTrackInfo(string name, string artist, string? release = null) {
    this.Artist = artist;
    this.Name = name;
    this.Release = release;
  }

  /// <inheritdoc cref="ISubmittedTrackInfo.AdditionalInfo"/>
  public Dictionary<string, object?>? AdditionalInfo { get; set; }

  IReadOnlyDictionary<string, object?>? ISubmittedTrackInfo.AdditionalInfo => this.AdditionalInfo;

  /// <inheritdoc/>
  public required string Artist { get; set; }

  /// <inheritdoc/>
  public required string Name { get; set; }

  /// <inheritdoc/>
  public string? Release { get; set; }

}
