using System;
using System.Collections.Generic;

using MetaBrainz.Common.Json;

namespace MetaBrainz.ListenBrainz.Interfaces.JSPF;

/// <summary>A JSPF playlist (with MusicBrainz extensions).</summary>
public interface IPlaylist : IJsonBasedObject {

  /// <summary>A comment associated with the playlist.</summary>
  string? Annotation { get; }

  /// <summary>A list of named URIs, intended to satisfy licenses that allow modification but require attribution.</summary>
  IReadOnlyList<INamedUri>? Attribution { get; }

  /// <summary>The name of the entity that authored the playlist.</summary>
  string? Creator { get; }

  /// <summary>The creation date of the playlist.</summary>
  DateTimeOffset? Date { get; }

  /// <summary>
  /// Additional arbitrary data associated with this playlist.<br/>
  /// The key is a URI identifying the application that created the data; the value is a list of arbitrary elements.
  /// </summary>
  IReadOnlyDictionary<Uri, IReadOnlyList<object?>?>? Extensions { get; }

  /// <summary>The canonical ID for this playlist.</summary>
  Uri? Identifier { get; }

  /// <summary>URI of an image associated with this playlist.</summary>
  Uri? Image { get; }

  /// <summary>URI of a web page where one can find out more about this playlist.</summary>
  Uri? Info { get; }

  /// <summary>The license under which this playlist was released.</summary>
  Uri? License { get; }

  /// <summary>Links to resources associated with the playlist.</summary>
  IReadOnlyList<ILink>? Links { get; }

  /// <summary>Source URI for this playlist.</summary>
  Uri? Location { get; }

  /// <summary>Metadata associated with the playlist.</summary>
  IReadOnlyList<IMeta>? Metadata { get; }

  /// <summary>MusicBrainz-specific information about the playlist.</summary>
  IMusicBrainzPlaylist? MusicBrainz { get; }

  /// <summary>The playlist's title.</summary>
  string? Title { get; }

  /// <summary>The tracks included in the playlist.</summary>
  IReadOnlyList<ITrack> Tracks { get; }

}
