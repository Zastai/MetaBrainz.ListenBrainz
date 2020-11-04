using System;
using System.Collections.Generic;

using JetBrains.Annotations;

namespace MetaBrainz.ListenBrainz.Interfaces {

  /// <summary>Statistical information about a release.</summary>
  [PublicAPI]
  public interface IReleaseInfo {

    /// <summary>The MusicBrainz IDs for the release's artist(s), if available.</summary>
    IReadOnlyList<Guid>? ArtistIds { get; }

    /// <summary>The MessyBrainz ID for the release's artist, if available.</summary>
    Guid? ArtistMessyId { get; }

    /// <summary>The release's artist's name, if available.</summary>
    string? ArtistName { get; }

    /// <summary>The MusicBrainz ID for the release, if available.</summary>
    Guid? Id { get; }

    /// <summary>The number of times the release's tracks were listened to.</summary>
    int ListenCount { get; }

    /// <summary>The MessyBrainz ID for the release, if available.</summary>
    Guid? MessyId { get; }

    /// <summary>The release's name.</summary>
    string Name { get; }

  }

}
