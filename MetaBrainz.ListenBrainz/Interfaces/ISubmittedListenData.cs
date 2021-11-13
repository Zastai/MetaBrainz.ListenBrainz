using JetBrains.Annotations;

namespace MetaBrainz.ListenBrainz.Interfaces;

/// <summary>Information about a single listen.</summary>
[PublicAPI]
public interface ISubmittedListenData {

  /// <summary>Information about the track that was listened to.</summary>
  ISubmittedTrackInfo Track { get; }

}
