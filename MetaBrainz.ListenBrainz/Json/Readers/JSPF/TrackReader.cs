using System;
using System.Collections.Generic;
using System.Text.Json;

using MetaBrainz.Common.Json;
using MetaBrainz.Common.Json.Converters;
using MetaBrainz.ListenBrainz.Interfaces.JSPF;
using MetaBrainz.ListenBrainz.Objects.JSPF;

namespace MetaBrainz.ListenBrainz.Json.Readers.JSPF;

internal class TrackReader : ObjectReader<Track> {

  private static readonly Uri ExtensionDataUri = new("https://musicbrainz.org/doc/jspf#track");

  public static readonly TrackReader Instance = new();

  private static readonly Uri OldExtensionDataUri = new("https://musicbrainz.org/recording/");

  protected override Track ReadObjectContents(ref Utf8JsonReader reader, JsonSerializerOptions options) {
    string? album = null;
    string? annotation = null;
    string? creator = null;
    uint? duration = null;
    IReadOnlyDictionary<Uri, IReadOnlyList<object?>?>? extensions = null;
    IReadOnlyList<Uri>? identifiers = null;
    Uri? image = null;
    Uri? info = null;
    IReadOnlyList<ILink>? links = null;
    IReadOnlyList<Uri>? locations = null;
    IReadOnlyList<IMeta>? meta = null;
    IMusicBrainzTrack? musicBrainz = null;
    IMusicBrainzRecording? musicBrainzRecording = null;
    string? title = null;
    uint? trackNumber = null;
    Dictionary<string, object?>? rest = null;
    Dictionary<Uri, Helpers.ReadExtensionData> knownExtensions = new() {
      { TrackReader.ExtensionDataUri, ReadExtensionData },
      { TrackReader.OldExtensionDataUri, ReadOldExtensionData },
    };
    while (reader.TokenType == JsonTokenType.PropertyName) {
      var prop = reader.GetPropertyName();
      try {
        reader.Read();
        switch (prop) {
          case "album":
            album = reader.GetString();
            break;
          case "annotation":
            annotation = reader.GetString();
            break;
          case "creator":
            creator = reader.GetString();
            break;
          case "duration":
            duration = reader.GetUInt32();
            break;
          case "image":
            image = reader.GetOptionalUri();
            break;
          case "info":
            info = reader.GetOptionalUri();
            break;
          case "extension":
            extensions = reader.ReadExtensions(options, knownExtensions);
            break;
          case "identifier":
            if (reader.TokenType == JsonTokenType.String) {
              // LB-1836: this is a violation of the JSPF spec, but it's what ListenBrainz currently does.
              identifiers = [ reader.GetUri() ];
            }
            else {
              identifiers = reader.ReadList<Uri>(options);
            }
            break;
          case "link":
            links = reader.ReadList(LinkReader.Instance, options);
            break;
          case "location":
            locations = reader.ReadList<Uri>(options);
            break;
          case "meta":
            meta = reader.ReadList(MetaReader.Instance, options);
            break;
          case "title":
            title = reader.GetString();
            break;
          case "trackNum":
            trackNumber = reader.GetUInt32();
            break;
          default:
            rest ??= [ ];
            rest[prop] = reader.GetOptionalObject(options);
            break;
        }
      }
      catch (Exception e) {
        throw new JsonException($"Failed to deserialize the '{prop}' property.", e);
      }
      reader.Read();
    }
    return new Track {
      Album = album,
      Annotation = annotation,
      Creator = creator,
      Duration = duration is null ? null : TimeSpan.FromMilliseconds(duration.Value),
      Extensions = extensions,
      Identifiers = identifiers,
      Image = image,
      Info = info,
      Links = links,
      Locations = locations,
      Metadata = meta,
      MusicBrainz = musicBrainz,
      MusicBrainzRecording = musicBrainzRecording,
      Title = title,
      TrackNumber = trackNumber,
      UnhandledProperties = rest,
    };
    bool ReadExtensionData(ref Utf8JsonReader r, JsonSerializerOptions o) {
      musicBrainz = r.GetOptionalObject(MusicBrainzTrackReader.Instance, o);
      return true;
    }
    bool ReadOldExtensionData(ref Utf8JsonReader r, JsonSerializerOptions o) {
      musicBrainzRecording = r.GetOptionalObject(MusicBrainzRecordingReader.Instance, o);
      return true;
    }
  }

}
