using System;
using System.Collections.Generic;

using JetBrains.Annotations;

using MetaBrainz.Common.Json;

namespace MetaBrainz.ListenBrainz.Interfaces;

/// <summary>Statistical information about a release group.</summary>
[PublicAPI]
public interface IReleaseGroupInfo : IJsonBasedObject {

  /// <summary>The MusicBrainz IDs for the release group's credited artist(s).</summary>
  IReadOnlyList<Guid>? ArtistIds { get; }

  /// <summary>
  /// The release group's credited artist name.<br/>
  /// This may combine multiple distinct artists; look at <see cref="Credits"/> for details.
  /// </summary>
  string? ArtistName { get; }

  /// <summary>The internal ID for the release group in the CoverArt Archive.</summary>
  long? CoverArtId { get; }

  /// <summary>The MusicBrainz ID for the release group in the CoverArt Archive.</summary>
  Guid? CoverArtReleaseGroupId { get; }

  /// <summary>The MusicBrainz credits for the release group's artists.</summary>
  IReadOnlyList<IArtistCredit>? Credits { get; }

  /// <summary>The MusicBrainz ID for the release group.</summary>
  Guid? Id { get; }

  /// <summary>The number of times the release group's tracks were listened to.</summary>
  int ListenCount { get; }

  /// <summary>The release group's name.</summary>
  string Name { get; }

}
