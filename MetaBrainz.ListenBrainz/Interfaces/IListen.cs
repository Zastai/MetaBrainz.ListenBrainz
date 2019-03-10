using System;

namespace MetaBrainz.ListenBrainz.Interfaces {

  /// <summary>Information about a single listen.</summary>
  public interface IListen {

    /// <summary>The MessyBrainz ID for the recording that was listened to.</summary>
    Guid? MessyRecording { get; }

    /// <summary>The timestamp for the listen.</summary>
    DateTime Timestamp { get; }

    /// <summary>Information about the track that was listened to.</summary>
    ITrackMetaData Track { get; }

    /// <summary>The timestamp for the listen.</summary>
    long UnixTimestamp { get; }

  }

}
