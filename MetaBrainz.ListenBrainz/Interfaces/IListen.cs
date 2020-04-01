using System;

using JetBrains.Annotations;

using MetaBrainz.Common.Json;

namespace MetaBrainz.ListenBrainz.Interfaces {

  /// <summary>Information about a single listen.</summary>
  [PublicAPI]
  public interface IListen : IJsonBasedObject {

    /// <summary>The MessyBrainz ID for the recording that was listened to.</summary>
    Guid? MessyRecordingId { get; }

    /// <summary>The timestamp for the listen.</summary>
    DateTime? Timestamp { get; }

    /// <summary>Information about the track that was listened to.</summary>
    ITrackInfo? Track { get; }

    /// <summary>The MusicBrainz ID of the user who submitted the listen.</summary>
    string? User { get; }

  }

}
