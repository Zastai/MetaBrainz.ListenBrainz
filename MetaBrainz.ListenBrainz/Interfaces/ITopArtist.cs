using System;

using MetaBrainz.Common.Json;

namespace MetaBrainz.ListenBrainz.Interfaces;

/// <summary>Information about one of a user's most listened to artists.</summary>
public interface ITopArtist : IJsonBasedObject {

  /// <summary>The MusicBrainz ID for the artist, if available.</summary>
  Guid? Id { get; }

  /// <summary>The number of times the artist was listened to.</summary>
  int ListenCount { get; }

  /// <summary>The artist's name.</summary>
  string Name { get; }

}
