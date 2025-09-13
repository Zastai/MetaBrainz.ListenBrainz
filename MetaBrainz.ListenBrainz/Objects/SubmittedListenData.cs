using System;
using System.Diagnostics.CodeAnalysis;

using JetBrains.Annotations;

using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

/// <inheritdoc cref="ISubmittedListenData"/>
[PublicAPI]
public class SubmittedListenData : ISubmittedListenData {

  /// <summary>Creates new listen data.</summary>
  /// <remarks>
  /// This constructor is only here to enable a plain object initializer and will be removed once the other constructor is.
  /// </remarks>
  public SubmittedListenData() { }

  /// <summary>Creates new listen data.</summary>
  /// <param name="track">The listened track's name.</param>
  /// <param name="artist">The listened track's artist.</param>
  /// <param name="release">The listened track's release.</param>
  [Obsolete("Use an object initializer to set the track info.")]
  [SetsRequiredMembers]
  public SubmittedListenData(string track, string artist, string? release = null) {
    this.Track = new SubmittedTrackInfo {
      Artist = artist,
      Name = track,
      Release = release,
    };
  }

  /// <inheritdoc cref="ISubmittedListenData.Track"/>
  public required ISubmittedTrackInfo Track { get; init; }

}
