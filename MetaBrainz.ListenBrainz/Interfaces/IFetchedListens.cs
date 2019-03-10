using System.Collections.Generic;

namespace MetaBrainz.ListenBrainz.Interfaces {

  /// <summary>Information about a set of fetched listens.</summary>
  public interface IFetchedListens {

    /// <summary>The number of listens fetched.</summary>
    int Count { get; }

    /// <summary>The listens that were fetched.</summary>
    IReadOnlyList<IListen> Listens { get; }

    /// <summary>The MusicBrainz ID of the user for which the listens were fetched.</summary>
    string User { get; }

  }

}
