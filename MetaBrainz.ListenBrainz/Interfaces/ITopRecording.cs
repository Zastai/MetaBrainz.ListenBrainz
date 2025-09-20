using System;
using System.Collections.Generic;

using MetaBrainz.Common.Json;

namespace MetaBrainz.ListenBrainz.Interfaces;

/// <summary>Information about one of a user's most listened to recordings (tracks).</summary>
public interface ITopRecording : IJsonBasedObject {

  /// <summary>The MusicBrainz IDs for the recording's credited artist(s).</summary>
  IReadOnlyList<Guid>? ArtistIds { get; }

  /// <summary>
  /// The recording's credited artist name.<br/>
  /// This may combine multiple distinct artists; look at <see cref="Credits"/> for details.
  /// </summary>
  string? ArtistName { get; }

  /// <summary>The internal ID for the recording in the CoverArt Archive.</summary>
  /// <remarks>This field is only provided for the Year in Music 2022 and later.</remarks>
  long? CoverArtId { get; }

  /// <summary>The MusicBrainz release ID for the recording in the CoverArt Archive.</summary>
  /// <remarks>This field is only provided for the Year in Music 2022 and later.</remarks>
  Guid? CoverArtReleaseId { get; }

  /// <summary>The MusicBrainz credits for the recording's artists.</summary>
  /// <remarks>This field is only provided for the Year in Music 2023 and later.</remarks>
  IReadOnlyList<IArtistCredit>? Credits { get; }

  /// <summary>The MusicBrainz ID for the recording.</summary>
  Guid? Id { get; }

  /// <summary>The number of times the recording was listened to.</summary>
  int ListenCount { get; }

  /// <summary>The recording's name.</summary>
  string Name { get; }

  /// <summary>The MusicBrainz ID for the recording's release.</summary>
  Guid? ReleaseId { get; }

  /// <summary>The name of the recording's release.</summary>
  string? ReleaseName { get; }

}
