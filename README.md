# MetaBrainz.ListenBrainz [![Build Status](https://img.shields.io/appveyor/build/zastai/metabrainz-listenbrainz)](https://ci.appveyor.com/project/Zastai/metabrainz-listenbrainz) [![NuGet Version](https://img.shields.io/nuget/v/MetaBrainz.ListenBrainz)](https://www.nuget.org/packages/MetaBrainz.ListenBrainz)

This is a library providing access to the [ListenBrainz API v1](https://listenbrainz.readthedocs.io/en/latest/dev/api.html).

[ListenBrainz](https://listenbrainz.org/) keeps track of users' listens of music tracks (similar to sites like
[last.fm](https://www.last.fm) and [libre.fm](https://libre.fm)).

## Release Notes

### v2.1.0 (not yet released)

#### API Additions

- Method: `ListenBrainz.GetListensBetween()` and `ListenBrainz.GetListensBetweenAsync()` 

#### API Removals

#### API Changes

#### Other Changes

#### Dependency Updates

- MetaBrainz.Common.Json → 3.0.1
- System.Text.Json → 4.7.2


### v2.0.0 (2020-04-27)

This release contains a completely rewritten JSON backend, using custom converters.
There are also some breaking API changes (hence the major version bump).

#### API Additions

- Interface: `IPlayingNow`
- Interface: `IPlayingTrack`
- Interface: `IRecentListens`
- Property: `IAdditionalInfo.ImportedArtistId`
- Property: `IAdditionalInfo.ImportedReleaseId`

#### API Removals

- `IFetchedListens`:
  - this was being used for 3 distinct use cases, two of which have now been split out to separate interfaces: `IPlayingNow`
    (returned by `ListenBrainz.GetPlayingNow()`) and `IRecentListens` (returned by `ListenBrainz.GetRecentListens()`)
  - as a result, the `Count`, `PlayingNow` and `UserList` properties were removed
- `UnixTime.Converter` was dropped

#### API Changes

- `IAdditionalInfo`:
  - the `AllFields` property is no longer nullable, but its contained values are now nullable
  - the elements in the list properties (`ArtistIds`, `ArtistNames`, `ReleaseArtistNames`, `SpotifyAlbumArtistIds`, `SpotifyArtistIds`, `Tags` and `WorkIds`) are now nullable
- `IFetchedListens`:
  - none of the properties are nullable now
- `IListen`:
  - none of the properties are nullable now
- `ITrackInfo`:
  - the `AdditionalInfo`, `Artist` and `Name` properties are no longer nullable
- `ListenBrainz.GetListens()` and `ListenBrainz.GetListensAsync()`:
  - the overloads taking `after` and `before` parameters were dropped
    - they were a bit clunky in use, plus using both is not currently supported by the server API
  - instead, new companion methods were added:
    - `ListenBrainz.GetListensAfter()` and `ListenBrainz.GetListensAfterAsync()`
    - `ListenBrainz.GetListensBefore()` and `ListenBrainz.GetListensBeforeAsync()`
    - once [LB-518](https://tickets.metabrainz.org/browse/LB-518) is fixed, a future release will also provide
      `ListenBrainz.GetListensBetween()` and `ListenBrainz.GetListensBetweenAsync()`
- `ListenBrainz.GetPlayingNow()` and `ListenBrainz.GetPlayingNowAsync()` now return an `IPlayingNow` object
- `ListenBrainz.GetRecentListens()` and `ListenBrainz.GetRecentListensAsync()` now return an `IRecentListens` object

#### Other Changes

- a build issue that prevented the API documentation from being included in the package has been resolved

#### Dependency Updates

- JetBrainz.Annotations → 2020.1.0
- MetaBrainz.Common.Json → 3.0.0
- System.Text.Json → 4.7.1


### v1.0.0 (2020-04-04)

First release.

This package contains classes for accessing the ListenBrainz API endpoints.
It supports both retrieval of existing listens and submitting new data.
