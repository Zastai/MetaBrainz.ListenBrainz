using System;
using System.Collections.Generic;

namespace MetaBrainz.ListenBrainz.Interfaces.JSPF;

/// <summary>Additional information for a playlist, as defined by MusicBrainz.</summary>
/// <seealso href="https://musicbrainz.org/doc/jspf#playlist"/>
public interface IMusicBrainzPlaylist {

  /// <summary>
  /// Additional playlist metadata that may be used by playlist generation tools. The contents are defined by those playlist
  /// generation tools.
  /// </summary>
  IReadOnlyDictionary<string, object?>? AdditionalMetadata { get; }

  /// <summary>Who are the ListenBrainz users who have access to edit this playlist?</summary>
  IReadOnlyList<string>? Collaborators { get; }

  /// <summary>If this playlist was copied from an existing playlist, this identifies that original playlist.</summary>
  Uri? CopiedFrom { get; }

  /// <summary>
  /// Indicates that this playlist was copied from an existing playlist, but the original playlist has been deleted, so
  /// <see cref="CopiedFrom"/> cannot be provided.
  /// </summary>
  bool? CopiedFromDeleted { get; }

  /// <summary>Which ListenBrainz user was the playlist generated for?</summary>
  /// <remarks>This is for music recommendation bots generating playlists for users.</remarks>
  string? CreatedFor { get; }

  /// <summary>The ListenBrainz user who created this playlist.</summary>
  string? Creator { get; }

  /// <summary>The timestamp for when this playlist was last modified.</summary>
  DateTimeOffset? LastModified { get; }

  /// <summary>Indicates whether this playlist is public or private.</summary>
  bool? Public { get; }

}
