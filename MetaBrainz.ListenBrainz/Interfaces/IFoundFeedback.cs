using System.Collections.Generic;

using MetaBrainz.Common.Json;

namespace MetaBrainz.ListenBrainz.Interfaces;

/// <summary>The results of a feedback search.</summary>
public interface IFoundFeedback : IJsonBasedObject {

  /// <summary>The maximum number of matches returned.</summary>
  public int Count { get; }

  /// <summary>The feedback items that were found (if any).</summary>
  public IReadOnlyList<IRecordingFeedback> Feedback { get; }

  /// <summary>The (0-based) offset of the returned feedback items from the start of the full set.</summary>
  public int Offset { get; }

  /// <summary>The total number of matching feedback items.</summary>
  public int TotalCount { get; }

}
