using System;
using System.Collections.Generic;

using MetaBrainz.Common.Json;

namespace MetaBrainz.ListenBrainz.Interfaces;

/// <summary>Information about one of a user's most listened to releases.</summary>
public interface ITopRelease : IJsonBasedObject {

  /// <summary>The MusicBrainz IDs for the release's credited artist(s).</summary>
  IReadOnlyList<Guid>? ArtistIds { get; }

  /// <summary>The release's credited artist name.</summary>
  string? ArtistName { get; }

  /// <summary>The internal ID for the release in the CoverArt Archive.</summary>
  /// <remarks>This field is only provided for the Year in Music 2022.</remarks>
  long? CoverArtId { get; }

  /// <summary>The MusicBrainz ID for the release in the CoverArt Archive.</summary>
  /// <remarks>This field is only provided for the Year in Music 2022.</remarks>
  Guid? CoverArtReleaseGroupId { get; }

  /// <summary>The MusicBrainz ID for the releaserest ??= [ ];.</summary>
  Guid? Id { get; }

  /// <summary>The number of times the release's tracks were listened to.</summary>
  int ListenCount { get; }

  /// <summary>The release's name.</summary>
  string Name { get; }

}
