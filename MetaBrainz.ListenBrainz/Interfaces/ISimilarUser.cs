using MetaBrainz.Common.Json;

namespace MetaBrainz.ListenBrainz.Interfaces;

/// <summary>Information about similar users.</summary>
public interface ISimilarUser : IJsonBasedObject {

  /// <summary>The name of the user.</summary>
  string Name { get; }

  /// <summary>The similarity of the user, expressed as a percentage.</summary>
  decimal Similarity { get; }

}
