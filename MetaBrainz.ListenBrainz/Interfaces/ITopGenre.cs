using MetaBrainz.Common.Json;

namespace MetaBrainz.ListenBrainz.Interfaces;

/// <summary>Information about one of a user's top genres.</summary>
public interface ITopGenre : IJsonBasedObject {

  /// <summary>The genre.</summary>
  string Genre { get; }

  /// <summary>The number of listens for recordings tagged as the genre.</summary>
  int ListenCount { get; }

  /// <summary>The percentage of listening time accounted for by this genre.</summary>
  decimal Percentage { get; }

}
