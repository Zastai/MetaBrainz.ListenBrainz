using System.Collections.Generic;

using JetBrains.Annotations;

using MetaBrainz.Common.Json;

namespace MetaBrainz.ListenBrainz.Interfaces;

/// <summary>Information about recent listens for a set of users.</summary>
[PublicAPI]
public interface IRecentListens : IJsonBasedObject {

  /// <summary>The recent listens for the users listed in <see cref="UserList"/>.</summary>
  IReadOnlyList<IListen> Listens { get; }

  /// <summary>A comma-separated list of the MusicBrainz IDs of the users for which the listens were fetched.</summary>
  string UserList { get; }

}
