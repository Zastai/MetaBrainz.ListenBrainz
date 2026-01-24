using System;

using JetBrains.Annotations;

using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

/// <inheritdoc />
[PublicAPI]
public sealed class SubmittedRecordingFeedback : ISubmittedRecordingFeedback {

  /// <inheritdoc />
  public Guid? Id { get; set; }

  /// <inheritdoc />
  public Guid? MessyId { get; set; }

  /// <inheritdoc />
  public int Score {
    get;
    set {
      if (value is < -1 or > 1) {
        throw new ArgumentOutOfRangeException(nameof(this.Score), value,
                                              "The score must be either -1 (hate), 0 (remove feedback) or 1 (love).");
      }
      field = value;
    }
  }

}
