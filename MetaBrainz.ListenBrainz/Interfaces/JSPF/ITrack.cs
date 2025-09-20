using System;
using System.Collections.Generic;

using MetaBrainz.Common.Json;

namespace MetaBrainz.ListenBrainz.Interfaces.JSPF;

/// <summary>Information about a track in a JSPF playlist (with MusicBrainz extensions).</summary>
public interface ITrack : IJsonBasedObject {

  /// <summary>The name of the collection from which the track was taken.</summary>
  string? Album { get; }

  /// <summary>A comment associated with the track.</summary>
  string? Annotation { get; }

  /// <summary>The name of the entity that authored the track.</summary>
  string? Creator { get; }

  /// <summary>The duration of the track.</summary>
  TimeSpan? Duration { get; }

  /// <summary>
  /// Additional arbitrary data associated with this playlist.<br/>
  /// The key is a URI identifying the application that created the data; the value is a list of arbitrary elements.
  /// </summary>
  IReadOnlyDictionary<Uri, IReadOnlyList<object?>?>? Extensions { get; }

  /// <summary>Canonical identifiers for this track.</summary>
  IReadOnlyList<Uri>? Identifiers { get; }

  /// <summary>URI of an image associated with this playlist.</summary>
  Uri? Image { get; }

  /// <summary>URI of a web page where one can purchase and/or find out more about this track.</summary>
  Uri? Info { get; }

  /// <summary>Links to resources associated with the track.</summary>
  IReadOnlyList<ILink>? Links { get; }

  /// <summary>
  /// A number of URIs where the track can be found (e.g. URLs pointing to lossless and lossy versions of the audio).
  /// </summary>
  IReadOnlyList<Uri>? Locations { get; }

  /// <summary>Metadata associated with the track.</summary>
  IReadOnlyList<IMeta>? Metadata { get; }

  /// <summary>MusicBrainz-specific information about the track.</summary>
  IMusicBrainzTrack? MusicBrainz { get; }

  /// <summary>MusicBrainz-specific information about the track.</summary>
  /// <remarks>This is only returned for old playlists, like those that are part of the Year in Music data for 2021.</remarks>
  IMusicBrainzRecording? MusicBrainzRecording { get; }

  /// <summary>The track title.</summary>
  string? Title { get; }

  /// <summary>The ordinal position (with 1 being the first element) of the track within <see cref="Album"/>.</summary>
  uint? TrackNumber { get; }

}
