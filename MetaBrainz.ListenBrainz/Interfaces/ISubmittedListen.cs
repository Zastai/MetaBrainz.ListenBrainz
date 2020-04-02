using System;

using JetBrains.Annotations;

namespace MetaBrainz.ListenBrainz.Interfaces {

  /// <summary>Information about a single listen, including a timestamp.</summary>
  [PublicAPI]
  public interface ISubmittedListen : ISubmittedListenData {

    /// <summary>The timestamp for the listen.</summary>
    DateTimeOffset Timestamp { get; }

  }

}
