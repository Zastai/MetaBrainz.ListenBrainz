using System;
using JetBrains.Annotations;

namespace MetaBrainz.ListenBrainz.Interfaces {

  /// <summary>Information about a user's most recent import of listen data.</summary>
  [PublicAPI]
  public interface ILatestImport : IJsonBasedObject {

    /// <summary>The timestamp of the newest listen submitted in previous imports.</summary>
    DateTime Timestamp { get; }

    /// <summary>The timestamp of the newest listen submitted in previous imports.</summary>
    long UnixTimestamp { get; }

    /// <summary>The MusicBrainz ID of the user.</summary>
    string User { get; }

  }

}
