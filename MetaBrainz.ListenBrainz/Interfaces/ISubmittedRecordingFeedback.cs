using System;

namespace MetaBrainz.ListenBrainz.Interfaces;

/// <summary>Feedback to submit for a recording.</summary>
public interface ISubmittedRecordingFeedback {

  /// <summary>The MusicBrainz ID identifying the recording for which feedback is being provided.</summary>
  /// <remarks>When submitting, at least one of this property and <see cref="MessyId"/> must be set.</remarks>
  public Guid? Id { get; }

  /// <summary>The MessyBrainz ID identifying the recording for which feedback is being provided.</summary>
  /// <remarks>When submitting, at least one of this property and <see cref="Id"/> must be set.</remarks>
  public Guid? MessyId { get; }

  /// <summary>
  /// The score to submit.<br/>
  /// This must be one of the following values:
  /// <list type="table">
  ///   <listheader>
  ///     <term>Score</term>
  ///     <description>Action</description>
  ///   </listheader>
  ///   <item>
  ///     <term>0</term>
  ///     <description>Remove existing feedback.</description>
  ///   </item>
  ///   <item>
  ///     <term>1</term>
  ///     <description>Mark recording as loved.</description>
  ///   </item>
  ///   <item>
  ///     <term>-1</term>
  ///     <description>Mark recording as hated.</description>
  ///   </item>
  /// </list>
  /// </summary>
  public int Score { get; }

}
