using JetBrains.Annotations;

using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

/// <inheritdoc cref="ISubmittedListenData"/>
[PublicAPI]
public class SubmittedListenData : ISubmittedListenData {

  /// <inheritdoc cref="ISubmittedListenData.Track"/>
  public required ISubmittedTrackInfo Track { get; init; }

}
