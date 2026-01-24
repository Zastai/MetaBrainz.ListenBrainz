using System.Collections.Generic;

using JetBrains.Annotations;

using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

/// <inheritdoc cref="ISubmittedTrackInfo"/>
[PublicAPI]
public class SubmittedTrackInfo : ISubmittedTrackInfo {

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
