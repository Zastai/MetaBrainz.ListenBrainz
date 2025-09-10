using System;
using System.Collections.Generic;

using MetaBrainz.Common.Json;

namespace MetaBrainz.ListenBrainz.Interfaces;

/// <summary>Listening statistics for a single artist.</summary>
public interface IArtistActivityInfo : IJsonBasedObject {

  /// <summary>Listening statistics for the artist's albums.</summary>
  IReadOnlyList<IAlbumInfo> Albums { get; }

  /// <summary>The MusicBrainz ID for the artist, if available.</summary>
  Guid? Id { get; }

  /// <summary>The number of times the artist was listened to.</summary>
  int ListenCount { get; }

  /// <summary>The artist's name.</summary>
  string Name { get; }

}
