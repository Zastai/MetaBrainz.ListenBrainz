using System;
using System.Collections.Generic;

using JetBrains.Annotations;

using MetaBrainz.Common.Json;

namespace MetaBrainz.ListenBrainz.Interfaces;

/// <summary>Statistical information about an artist.</summary>
[PublicAPI]
public interface IArtistInfo : IJsonBasedObject {

  /// <summary>The MusicBrainz IDs for the artist, if available.</summary>
  IReadOnlyList<Guid>? Ids { get; }

  /// <summary>The number of times the artist's tracks were listened to.</summary>
  int ListenCount { get; }

  /// <summary>The MessyBrainz ID for the artist, if available.</summary>
  Guid? MessyId { get; }

  /// <summary>The artist's name.</summary>
  string Name { get; }

}
