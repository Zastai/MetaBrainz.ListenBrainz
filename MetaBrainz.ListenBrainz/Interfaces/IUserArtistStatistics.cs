using System.Collections.Generic;

using JetBrains.Annotations;

namespace MetaBrainz.ListenBrainz.Interfaces {

  /// <summary>A user's most-listened artists.</summary>
  [PublicAPI]
  public interface IUserArtistStatistics : IUserStatistics {

    /// <summary>Information about the artists.</summary>
    IReadOnlyList<IArtistInfo>? Artists { get; }

    /// <summary>The total number of (distinct) artists listened to, if available.</summary>
    public int? TotalArtistCount { get; }

  }

}
