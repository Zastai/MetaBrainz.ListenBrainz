using JetBrains.Annotations;

using MetaBrainz.Common.Json;

namespace MetaBrainz.ListenBrainz.Interfaces {

  /// <summary>Information about a user's listen count.</summary>
  [PublicAPI]
  public interface IListenCount : IJsonBasedObject {

    /// <summary>The listen count.</summary>
    long Count { get; }

  }

}
