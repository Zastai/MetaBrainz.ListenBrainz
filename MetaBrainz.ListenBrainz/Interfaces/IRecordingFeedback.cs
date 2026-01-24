using System;

using MetaBrainz.Common.Json;

namespace MetaBrainz.ListenBrainz.Interfaces;

/// <summary>Feedback given to a recording.</summary>
public interface IRecordingFeedback : IJsonBasedObject {

  /// <summary>The timestamp for the creation of this feedback.</summary>
  public DateTimeOffset Created { get; }

  /// <summary>The MusicBrainz ID identifying the recording.</summary>
  public Guid? Id { get; }

  /// <summary>The MessyBrainz ID identifying the recording.</summary>
  public Guid? MessyId { get; }

  /// <summary>The feedback score assigned by the user. 1 means they loved the recording; -1 means they hated it.</summary>
  public int Score { get; }

  /// <summary>Extra metadata for the track.</summary>
  /// <remarks>
  /// This field is undocumented, but is currently returned by the feedback retrieval endpoints. It is expected to mostly (always?)
  /// be <see langword="null"/>.
  /// </remarks>
  public object? TrackMetadata { get; }

  /// <summary>The user who submitted this feedback.</summary>
  public string User { get; }

}
