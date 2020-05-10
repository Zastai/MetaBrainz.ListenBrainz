using MetaBrainz.Common.Json;

namespace MetaBrainz.ListenBrainz {

  /// <summary>The range covered by a set of user statistics.</summary>
  public enum StatisticsRange {

    /// <summary>The statistics cover all of a user's listens.</summary>
    AllTime,

    /// <summary>
    /// The range was specified as an unknown value; check the object's <see cref="IJsonBasedObject.UnhandledProperties"/> for the
    /// specific value used.
    /// </summary>
    Unknown,

  }

}
