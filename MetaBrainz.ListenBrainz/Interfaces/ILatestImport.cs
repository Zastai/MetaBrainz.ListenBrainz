using System;

namespace MetaBrainz.ListenBrainz.Interfaces {

  /// <summary>Information about a user's most recent import of listen data.</summary>
  public interface ILatestImport {

    /// <summary>The timestamp of the newest listen submitted in previous imports.</summary>
    DateTime Timestamp { get; }
    
    /// <summary>The MusicBrainz ID of the user.</summary>
    string User { get; }

  }

}
