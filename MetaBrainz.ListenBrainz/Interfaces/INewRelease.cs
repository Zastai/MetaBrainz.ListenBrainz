using System;
using System.Collections.Generic;

namespace MetaBrainz.ListenBrainz.Interfaces;

/// <summary>Information about a new release from a particular year.</summary>
public interface INewRelease {

  /// <summary>The internal ID for the release in the CoverArt Archive.</summary>
  /// <remarks>This field is only provided for the Year in Music 2022 and later.</remarks>
  long? CoverArtId { get; }

  /// <summary>The MusicBrainz ID for the release in the CoverArt Archive.</summary>
  /// <remarks>This field is only provided for the Year in Music 2022 and later.</remarks>
  Guid? CoverArtReleaseId { get; }

  /// <summary>
  /// The release's credited artist name.<br/>
  /// This may combine multiple distinct artists; look at <see cref="CreditedArtists"/> for details.
  /// </summary>
  /// <remarks>This field is only provided for the Year in Music 2023 and later.</remarks>
  string? CreditedArtist { get; }

  /// <summary>The MusicBrainz IDs for the release's credited artist(s).</summary>
  IReadOnlyList<Guid>? CreditedArtistIds { get; }

  /// <summary>The names of the release's credited artists.</summary>
  /// <remarks>
  /// This field is only provided for the Year in Music 2021; later versions provide <see cref="CreditedArtist"/> and (from 2023
  /// onwards) <see cref="CreditedArtists"/> instead.
  /// </remarks>
  IReadOnlyList<string>? CreditedArtistNames { get; }

  /// <summary>Information about the release's credited artists.</summary>
  /// <remarks>This field is only provided for the Year in Music 2023 and later.</remarks>
  IReadOnlyList<IArtistCredit>? CreditedArtists { get; }

  /// <summary>The date this was first released.</summary>
  /// <remarks>This field is only provided for the Year in Music 2021.</remarks>
  string? FirstReleaseDate { get; }

  /// <summary>The MusicBrainz release ID for the release.</summary>
  /// <remarks>This field is only provided for the Year in Music 2021.</remarks>
  Guid? ReleaseId { get; }

  /// <summary>The MusicBrainz release group ID for the release.</summary>
  /// <remarks>This field is only provided for the Year in Music 2022 and later.</remarks>
  Guid? ReleaseGroupId { get; }

  /// <summary>The release's title.</summary>
  string Title { get; }

  /// <summary>The type of release's title.</summary>
  /// <remarks>This field is only provided for the Year in Music 2021.</remarks>
  string? Type { get; }

}
