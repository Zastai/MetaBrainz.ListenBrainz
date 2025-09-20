using System;

using MetaBrainz.Common.Json;

namespace MetaBrainz.ListenBrainz.Interfaces;

/// <summary>Listening statistics for an album.</summary>
public interface IAlbumInfo : IJsonBasedObject {

  /// <summary>The MusicBrainz ID for the album's release group.</summary>
  Guid? Id { get; }

  /// <summary>The number of times the album was listened to.</summary>
  int ListenCount { get; }

  /// <summary>The album's title.</summary>
  string Title { get; }

}
