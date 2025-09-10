using MetaBrainz.Common.Json;

namespace MetaBrainz.ListenBrainz.Interfaces;

/// <summary>Information about a top listener.</summary>
public interface ITopListener : IJsonBasedObject {

  /// <summary>The listen count.</summary>
  int ListenCount { get; }

  /// <summary>The name of the listener.</summary>
  /// <remarks>
  /// When multiple users have the same listen count, this will contain all their names, separated by a comma and a blank.
  /// </remarks>
  string UserName { get; }

}
