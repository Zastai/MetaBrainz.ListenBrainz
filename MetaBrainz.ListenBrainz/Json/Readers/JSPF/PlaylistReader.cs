using System;
using System.Collections.Generic;
using System.Text.Json;

using MetaBrainz.Common.Json;
using MetaBrainz.Common.Json.Converters;
using MetaBrainz.ListenBrainz.Interfaces.JSPF;
using MetaBrainz.ListenBrainz.Objects.JSPF;

namespace MetaBrainz.ListenBrainz.Json.Readers.JSPF;

internal class PlaylistReader : ObjectReader<Playlist> {

  public static readonly PlaylistReader Instance = new();

  protected override Playlist ReadObjectContents(ref Utf8JsonReader reader, JsonSerializerOptions options) {
    Dictionary<string, object?>? rest = null;
    return this.ReadPlaylist(ref reader, options, ref rest);
  }

  private Playlist ReadObjectForPlaylist(ref Utf8JsonReader reader, JsonSerializerOptions options,
                                      ref Dictionary<string, object?>? rest) {
    if (reader.TokenType != JsonTokenType.StartObject) {
      throw new JsonException("Expected start of playlist object not found.");
    }
    reader.Read();
    var playlist = this.ReadPlaylist(ref reader, options, ref rest);
    if (reader.TokenType != JsonTokenType.EndObject) {
      throw new JsonException("Expected end of playlist data not found.");
    }
    return playlist;
  }

  private Playlist ReadPlaylist(ref Utf8JsonReader reader, JsonSerializerOptions options, ref Dictionary<string, object?>? rest) {
    if (reader.TokenType == JsonTokenType.PropertyName) {
      var prop = reader.GetPropertyName();
      // Check for the ListenBrainz envelope: { jspf: <playlist>, mbid: guid }
      if (prop is "jspf" or "mbid") {
        Playlist? playlist = null;
        while (reader.TokenType == JsonTokenType.PropertyName) {
          prop = reader.GetPropertyName();
          try {
            reader.Read();
            switch (prop) {
              case "jspf":
                playlist = this.ReadObjectForPlaylist(ref reader, options, ref rest);
                break;
              case "mbid":
                rest ??= [ ];
                rest["listenbrainz:mbid"] = reader.GetGuid();
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
        if (playlist is null) {
          throw new JsonException("Expected 'jspf' property not found.");
        }
        return playlist;
      }
      // Check for the outer JSPF layer: { playlist: <playlist> }
      if (prop is "playlist") {
        reader.Read();
        var playlist = this.ReadObjectForPlaylist(ref reader, options, ref rest);
        reader.Read();
        while (reader.TokenType == JsonTokenType.PropertyName) {
          prop = reader.GetPropertyName();
          try {
            reader.Read();
            rest ??= [ ];
            rest[$"playlist-wrapper:{prop}"] = reader.GetOptionalObject(options);
          }
          catch (Exception e) {
            throw new JsonException($"Failed to deserialize the '{prop}' property.", e);
          }
          reader.Read();
        }
        return playlist;
      }
    }
    string? annotation = null;
    IReadOnlyList<INamedUri>? attribution = null;
    DateTimeOffset? date = null;
    string? creator = null;
    IReadOnlyDictionary<Uri, IReadOnlyList<object?>?>? extensions = null;
    Uri? identifier = null;
    Uri? image = null;
    Uri? info = null;
    Uri? license = null;
    IReadOnlyList<ILink>? links = null;
    Uri? location = null;
    IReadOnlyList<IMeta>? meta = null;
    string? title = null;
    IReadOnlyList<ITrack>? tracks = null;
    while (reader.TokenType == JsonTokenType.PropertyName) {
      var prop = reader.GetPropertyName();
      try {
        reader.Read();
        switch (prop) {
          case "annotation":
            annotation = reader.GetString();
            break;
          case "attribution":
            attribution = reader.ReadList(NamedUriReader.Instance, options);
            break;
          case "creator":
            creator = reader.GetString();
            break;
          case "date":
            date = reader.GetDateTimeOffset();
            break;
          case "extension":
            extensions = reader.ReadExtensions(options);
            break;
          case "identifier":
            identifier = reader.GetOptionalUri();
            break;
          case "image":
            image = reader.GetOptionalUri();
            break;
          case "info":
            info = reader.GetOptionalUri();
            break;
          case "license":
            license = reader.GetOptionalUri();
            break;
          case "link":
            links = reader.ReadList(LinkReader.Instance, options);
            break;
          case "location":
            location = reader.GetOptionalUri();
            break;
          case "meta":
            meta = reader.ReadList(MetaReader.Instance, options);
            break;
          case "title":
            title = reader.GetString();
            break;
          case "track":
            tracks = reader.ReadList(TrackReader.Instance, options);
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
    return new Playlist {
      Annotation = annotation,
      Attribution = attribution,
      Date = date,
      Creator = creator,
      Extensions = extensions,
      Identifier = identifier,
      Image = image,
      Info = info,
      License = license,
      Links = links,
      Location = location,
      Metadata = meta,
      Title = title,
      Tracks = tracks ?? [ ],
      UnhandledProperties = rest,
    };
  }

}
