# API Reference: MetaBrainz.ListenBrainz

## Assembly Attributes

```cs
[assembly: System.Runtime.InteropServices.ComVisibleAttribute(false)]
[assembly: System.Runtime.Versioning.TargetFrameworkAttribute(".NETCoreApp,Version=v8.0", FrameworkDisplayName = ".NET 8.0")]
```

## Namespace: MetaBrainz.ListenBrainz

### Type: ListenBrainz

```cs
public sealed class ListenBrainz : System.IDisposable {

  public const int DefaultItemsPerGet = 25;

  public const int DefaultTimeRange = 3;

  public const int EarliestListen = 1033430400;

  public const int MaxDuration = 2073600;

  public const int MaxItemsPerGet = 100;

  public const int MaxListenPayloadSize = 10240000;

  public const int MaxListenSize = 10240;

  public const int MaxListensPerRequest = 1000;

  public const int MaxTagLength = 64;

  public const int MaxTagSize = 64;

  public const int MaxTagsPerListen = 50;

  public const int MaxTimeRange = 73;

  public static readonly System.Diagnostics.TraceSource TraceSource;

  public const string UserAgentUrl = "https://github.com/Zastai/MetaBrainz.ListenBrainz";

  public const string WebServiceRoot = "/1/";

  System.Uri BaseUri {
    public get;
  }

  int DefaultPort {
    public static get;
    public static set;
  }

  string DefaultServer {
    public static get;
    public static set;
  }

  string DefaultUrlScheme {
    public static get;
    public static set;
  }

  System.Collections.Generic.IList<System.Net.Http.Headers.ProductInfoHeaderValue> DefaultUserAgent {
    public static get;
  }

  string? DefaultUserToken {
    public static get;
    public static set;
  }

  int Port {
    public get;
    public set;
  }

  MetaBrainz.Common.RateLimitInfo RateLimitInfo {
    public get;
  }

  string Server {
    public get;
    public set;
  }

  string UrlScheme {
    public get;
    public set;
  }

  System.Collections.Generic.IList<System.Net.Http.Headers.ProductInfoHeaderValue> UserAgent {
    public get;
  }

  string? UserToken {
    public get;
    public set;
  }

  public ListenBrainz();

  public ListenBrainz(params System.Net.Http.Headers.ProductInfoHeaderValue[] userAgent);

  public ListenBrainz(System.Net.Http.HttpClient client, bool takeOwnership = false);

  public ListenBrainz(string application, System.Version? version);

  public ListenBrainz(string application, System.Version? version, System.Uri contact);

  public ListenBrainz(string application, System.Version? version, string contact);

  public ListenBrainz(string application, string? version);

  public ListenBrainz(string application, string? version, System.Uri contact);

  public ListenBrainz(string application, string? version, string contact);

  public void Close();

  public void ConfigureClient(System.Action<System.Net.Http.HttpClient>? code);

  public void ConfigureClientCreation(System.Func<System.Net.Http.HttpClient>? code);

  public sealed override void Dispose();

  protected override void Finalize();

  public System.Threading.Tasks.Task<MetaBrainz.ListenBrainz.Interfaces.IArtistActivity?> GetArtistActivityAsync(StatisticsRange? range = default, System.Threading.CancellationToken cancellationToken = default);

  public System.Threading.Tasks.Task<MetaBrainz.ListenBrainz.Interfaces.IArtistActivity?> GetArtistActivityAsync(string user, StatisticsRange? range = default, System.Threading.CancellationToken cancellationToken = default);

  public System.Threading.Tasks.Task<MetaBrainz.ListenBrainz.Interfaces.IArtistEvolutionActivity?> GetArtistEvolutionActivityAsync(StatisticsRange? range = default, System.Threading.CancellationToken cancellationToken = default);

  public System.Threading.Tasks.Task<MetaBrainz.ListenBrainz.Interfaces.IArtistEvolutionActivity?> GetArtistEvolutionActivityAsync(string user, StatisticsRange? range = default, System.Threading.CancellationToken cancellationToken = default);

  public System.Threading.Tasks.Task<MetaBrainz.ListenBrainz.Interfaces.IArtistListeners?> GetArtistListenersAsync(System.Guid mbid, int? count = default, int? offset = default, StatisticsRange? range = default, System.Threading.CancellationToken cancellationToken = default);

  public System.Threading.Tasks.Task<MetaBrainz.ListenBrainz.Interfaces.IArtistMap?> GetArtistMapAsync(StatisticsRange? range = default, System.Threading.CancellationToken cancellationToken = default);

  public System.Threading.Tasks.Task<MetaBrainz.ListenBrainz.Interfaces.IArtistMap?> GetArtistMapAsync(string user, StatisticsRange? range = default, System.Threading.CancellationToken cancellationToken = default);

  public System.Threading.Tasks.Task<MetaBrainz.ListenBrainz.Interfaces.IArtistStatistics?> GetArtistStatisticsAsync(int? count = default, int? offset = default, StatisticsRange? range = default, System.Threading.CancellationToken cancellationToken = default);

  public System.Threading.Tasks.Task<MetaBrainz.ListenBrainz.Interfaces.IArtistStatistics?> GetArtistStatisticsAsync(string user, int? count = default, int? offset = default, StatisticsRange? range = default, System.Threading.CancellationToken cancellationToken = default);

  public System.Threading.Tasks.Task<MetaBrainz.ListenBrainz.Interfaces.IUserDailyActivity?> GetDailyActivityAsync(string user, StatisticsRange? range = default, System.Threading.CancellationToken cancellationToken = default);

  public System.Threading.Tasks.Task<MetaBrainz.ListenBrainz.Interfaces.IEraActivity?> GetEraActivityAsync(StatisticsRange? range = default, System.Threading.CancellationToken cancellationToken = default);

  public System.Threading.Tasks.Task<MetaBrainz.ListenBrainz.Interfaces.IEraActivity?> GetEraActivityAsync(string user, StatisticsRange? range = default, System.Threading.CancellationToken cancellationToken = default);

  public System.Threading.Tasks.Task<MetaBrainz.ListenBrainz.Interfaces.IGenreActivity?> GetGenreActivityAsync(string user, StatisticsRange? range = default, System.Threading.CancellationToken cancellationToken = default);

  public System.Threading.Tasks.Task<MetaBrainz.ListenBrainz.Interfaces.ILatestImport> GetLatestImportAsync(string user, System.Threading.CancellationToken cancellationToken = default);

  public System.Threading.Tasks.Task<MetaBrainz.ListenBrainz.Interfaces.IListenCount> GetListenCountAsync(string user, System.Threading.CancellationToken cancellationToken = default);

  public System.Threading.Tasks.Task<MetaBrainz.ListenBrainz.Interfaces.IListeningActivity?> GetListeningActivityAsync(StatisticsRange? range = default, System.Threading.CancellationToken cancellationToken = default);

  public System.Threading.Tasks.Task<MetaBrainz.ListenBrainz.Interfaces.IListeningActivity?> GetListeningActivityAsync(string user, StatisticsRange? range = default, System.Threading.CancellationToken cancellationToken = default);

  public System.Threading.Tasks.Task<MetaBrainz.ListenBrainz.Interfaces.IFetchedListens> GetListensAfterAsync(string user, System.DateTimeOffset after, int? count = default, int? timeRange = default, System.Threading.CancellationToken cancellationToken = default);

  public System.Threading.Tasks.Task<MetaBrainz.ListenBrainz.Interfaces.IFetchedListens> GetListensAfterAsync(string user, long after, int? count = default, int? timeRange = default, System.Threading.CancellationToken cancellationToken = default);

  public System.Threading.Tasks.Task<MetaBrainz.ListenBrainz.Interfaces.IFetchedListens> GetListensAsync(string user, int? count = default, int? timeRange = default, System.Threading.CancellationToken cancellationToken = default);

  public System.Threading.Tasks.Task<MetaBrainz.ListenBrainz.Interfaces.IFetchedListens> GetListensBeforeAsync(string user, System.DateTimeOffset before, int? count = default, int? timeRange = default, System.Threading.CancellationToken cancellationToken = default);

  public System.Threading.Tasks.Task<MetaBrainz.ListenBrainz.Interfaces.IFetchedListens> GetListensBeforeAsync(string user, long before, int? count = default, int? timeRange = default, System.Threading.CancellationToken cancellationToken = default);

  public System.Threading.Tasks.Task<MetaBrainz.ListenBrainz.Interfaces.IFetchedListens> GetListensBetweenAsync(string user, System.DateTimeOffset after, System.DateTimeOffset before, int? count = default, System.Threading.CancellationToken cancellationToken = default);

  public System.Threading.Tasks.Task<MetaBrainz.ListenBrainz.Interfaces.IFetchedListens> GetListensBetweenAsync(string user, long after, long before, int? count = default, System.Threading.CancellationToken cancellationToken = default);

  public System.Threading.Tasks.Task<MetaBrainz.ListenBrainz.Interfaces.IPlayingNow> GetPlayingNowAsync(string user, System.Threading.CancellationToken cancellationToken = default);

  public System.Threading.Tasks.Task<MetaBrainz.ListenBrainz.Interfaces.IRecordingStatistics?> GetRecordingStatisticsAsync(int? count = default, int? offset = default, StatisticsRange? range = default, System.Threading.CancellationToken cancellationToken = default);

  public System.Threading.Tasks.Task<MetaBrainz.ListenBrainz.Interfaces.IRecordingStatistics?> GetRecordingStatisticsAsync(string user, int? count = default, int? offset = default, StatisticsRange? range = default, System.Threading.CancellationToken cancellationToken = default);

  public System.Threading.Tasks.Task<MetaBrainz.ListenBrainz.Interfaces.IReleaseGroupListeners?> GetReleaseGroupListenersAsync(System.Guid mbid, int? count = default, int? offset = default, StatisticsRange? range = default, System.Threading.CancellationToken cancellationToken = default);

  public System.Threading.Tasks.Task<MetaBrainz.ListenBrainz.Interfaces.IReleaseGroupStatistics?> GetReleaseGroupStatisticsAsync(int? count = default, int? offset = default, StatisticsRange? range = default, System.Threading.CancellationToken cancellationToken = default);

  public System.Threading.Tasks.Task<MetaBrainz.ListenBrainz.Interfaces.IReleaseGroupStatistics?> GetReleaseGroupStatisticsAsync(string user, int? count = default, int? offset = default, StatisticsRange? range = default, System.Threading.CancellationToken cancellationToken = default);

  public System.Threading.Tasks.Task<MetaBrainz.ListenBrainz.Interfaces.IReleaseStatistics?> GetReleaseStatisticsAsync(int? count = default, int? offset = default, StatisticsRange? range = default, System.Threading.CancellationToken cancellationToken = default);

  public System.Threading.Tasks.Task<MetaBrainz.ListenBrainz.Interfaces.IReleaseStatistics?> GetReleaseStatisticsAsync(string user, int? count = default, int? offset = default, StatisticsRange? range = default, System.Threading.CancellationToken cancellationToken = default);

  public System.Threading.Tasks.Task<MetaBrainz.ListenBrainz.Interfaces.IYearInMusic?> GetYearInMusicAsync(string user, System.Threading.CancellationToken cancellationToken = default);

  public System.Threading.Tasks.Task<MetaBrainz.ListenBrainz.Interfaces.IYearInMusic?> GetYearInMusicAsync(string user, int year, System.Threading.CancellationToken cancellationToken = default);

  public System.Threading.Tasks.Task ImportListensAsync(params MetaBrainz.ListenBrainz.Interfaces.ISubmittedListen[] listens);

  public System.Threading.Tasks.Task ImportListensAsync(System.Collections.Generic.IAsyncEnumerable<MetaBrainz.ListenBrainz.Interfaces.ISubmittedListen> listens, System.Threading.CancellationToken cancellationToken = default);

  public System.Threading.Tasks.Task ImportListensAsync(System.Collections.Generic.IEnumerable<MetaBrainz.ListenBrainz.Interfaces.ISubmittedListen> listens, System.Threading.CancellationToken cancellationToken = default);

  public System.Threading.Tasks.Task ImportListensAsync(System.Threading.CancellationToken cancellationToken, params MetaBrainz.ListenBrainz.Interfaces.ISubmittedListen[] listens);

  public System.Threading.Tasks.Task SetLatestImportAsync(string user, System.DateTimeOffset timestamp, System.Threading.CancellationToken cancellationToken = default);

  public System.Threading.Tasks.Task SetLatestImportAsync(string user, long timestamp, System.Threading.CancellationToken cancellationToken = default);

  public System.Threading.Tasks.Task SetNowPlayingAsync(MetaBrainz.ListenBrainz.Interfaces.ISubmittedListenData listen, System.Threading.CancellationToken cancellationToken = default);

  [System.ObsoleteAttribute("Create a SubmittedListenData and pass it to the overload taking an ISubmittedListenData instead.")]
  public System.Threading.Tasks.Task SetNowPlayingAsync(string track, string artist, string? release = null, System.Threading.CancellationToken cancellationToken = default);

  public System.Threading.Tasks.Task SubmitSingleListenAsync(MetaBrainz.ListenBrainz.Interfaces.ISubmittedListen listen, System.Threading.CancellationToken cancellationToken = default);

  [System.ObsoleteAttribute("Create a SubmittedListen and pass it to the overload taking an ISubmittedListen instead.")]
  public System.Threading.Tasks.Task SubmitSingleListenAsync(System.DateTimeOffset timestamp, string track, string artist, string? release = null, System.Threading.CancellationToken cancellationToken = default);

  [System.ObsoleteAttribute("Create a SubmittedListen and pass it to the overload taking an ISubmittedListen instead.")]
  public System.Threading.Tasks.Task SubmitSingleListenAsync(long timestamp, string track, string artist, string? release = null, System.Threading.CancellationToken cancellationToken = default);

  [System.ObsoleteAttribute("Create a SubmittedListen and pass it to the overload taking an ISubmittedListen instead.")]
  public System.Threading.Tasks.Task SubmitSingleListenAsync(string track, string artist, string? release = null, System.Threading.CancellationToken cancellationToken = default);

  public System.Threading.Tasks.Task<MetaBrainz.ListenBrainz.Interfaces.ITokenValidationResult> ValidateTokenAsync(string token, System.Threading.CancellationToken cancellationToken = default);

}
```

### Type: StatisticsRange

```cs
public enum StatisticsRange {

  AllTime = 0,
  HalfYearly = 1,
  Month = 2,
  Quarter = 3,
  ThisMonth = 4,
  ThisWeek = 5,
  ThisYear = 6,
  Unknown = 9,
  Week = 7,
  Year = 8,

}
```

## Namespace: MetaBrainz.ListenBrainz.Interfaces

### Type: IAdditionalInfo

```cs
public interface IAdditionalInfo {

  System.Collections.Generic.IReadOnlyDictionary<string, object?> AllFields {
    public abstract get;
  }

  System.Collections.Generic.IReadOnlyList<System.Guid?>? ArtistIds {
    public abstract get;
  }

  System.Collections.Generic.IReadOnlyList<string?>? ArtistNames {
    public abstract get;
  }

  int? DiscNumber {
    public abstract get;
  }

  System.TimeSpan? Duration {
    public abstract get;
  }

  System.Guid? ImportedArtistId {
    public abstract get;
  }

  System.Guid? ImportedReleaseId {
    public abstract get;
  }

  string? Isrc {
    public abstract get;
  }

  string? ListeningFrom {
    public abstract get;
  }

  string? MediaPlayer {
    public abstract get;
  }

  string? MediaPlayerVersion {
    public abstract get;
  }

  System.Guid? MessyArtistId {
    public abstract get;
  }

  System.Guid? MessyRecordingId {
    public abstract get;
  }

  System.Guid? MessyReleaseId {
    public abstract get;
  }

  string? MusicService {
    public abstract get;
  }

  string? MusicServiceName {
    public abstract get;
  }

  System.Uri? OriginUrl {
    public abstract get;
  }

  System.Guid? RecordingId {
    public abstract get;
  }

  string? ReleaseArtistName {
    public abstract get;
  }

  System.Collections.Generic.IReadOnlyList<string?>? ReleaseArtistNames {
    public abstract get;
  }

  System.Guid? ReleaseGroupId {
    public abstract get;
  }

  System.Guid? ReleaseId {
    public abstract get;
  }

  System.Collections.Generic.IReadOnlyList<System.Uri?>? SpotifyAlbumArtistIds {
    public abstract get;
  }

  System.Uri? SpotifyAlbumId {
    public abstract get;
  }

  System.Collections.Generic.IReadOnlyList<System.Uri?>? SpotifyArtistIds {
    public abstract get;
  }

  System.Uri? SpotifyId {
    public abstract get;
  }

  string? SubmissionClient {
    public abstract get;
  }

  string? SubmissionClientVersion {
    public abstract get;
  }

  System.Collections.Generic.IReadOnlyList<string?>? Tags {
    public abstract get;
  }

  System.Guid? TrackId {
    public abstract get;
  }

  int? TrackNumber {
    public abstract get;
  }

  System.Collections.Generic.IReadOnlyList<System.Guid?>? WorkIds {
    public abstract get;
  }

}
```

### Type: IAlbumInfo

```cs
public interface IAlbumInfo : MetaBrainz.Common.Json.IJsonBasedObject {

  System.Guid? Id {
    public abstract get;
  }

  int ListenCount {
    public abstract get;
  }

  string Title {
    public abstract get;
  }

}
```

### Type: IArtistActivity

```cs
public interface IArtistActivity : MetaBrainz.Common.Json.IJsonBasedObject {

  System.Collections.Generic.IReadOnlyList<IArtistActivityInfo> Artists {
    public abstract get;
  }

}
```

### Type: IArtistActivityInfo

```cs
public interface IArtistActivityInfo : MetaBrainz.Common.Json.IJsonBasedObject {

  System.Collections.Generic.IReadOnlyList<IAlbumInfo> Albums {
    public abstract get;
  }

  System.Guid? Id {
    public abstract get;
  }

  int ListenCount {
    public abstract get;
  }

  string Name {
    public abstract get;
  }

}
```

### Type: IArtistCountryInfo

```cs
public interface IArtistCountryInfo {

  int ArtistCount {
    public abstract get;
  }

  System.Collections.Generic.IReadOnlyList<IArtistInfo>? Artists {
    public abstract get;
  }

  string Country {
    public abstract get;
  }

  int ListenCount {
    public abstract get;
  }

}
```

### Type: IArtistCredit

```cs
public interface IArtistCredit {

  string CreditedName {
    public abstract get;
  }

  System.Guid Id {
    public abstract get;
  }

  string? JoinPhrase {
    public abstract get;
  }

}
```

### Type: IArtistEvolutionActivity

```cs
public interface IArtistEvolutionActivity : IStatistics, MetaBrainz.Common.Json.IJsonBasedObject {

  System.Collections.Generic.IReadOnlyList<IArtistTimeRange>? Activity {
    public abstract get;
  }

}
```

### Type: IArtistInfo

```cs
public interface IArtistInfo : MetaBrainz.Common.Json.IJsonBasedObject {

  System.Guid? Id {
    public abstract get;
  }

  System.Collections.Generic.IReadOnlyList<System.Guid>? Ids {
    public abstract get;
  }

  int ListenCount {
    public abstract get;
  }

  System.Guid? MessyId {
    public abstract get;
  }

  string Name {
    public abstract get;
  }

}
```

### Type: IArtistListeners

```cs
public interface IArtistListeners : IListenerInfo, IStatistics, MetaBrainz.Common.Json.IJsonBasedObject {

  System.Guid Id {
    public abstract get;
  }

  string Name {
    public abstract get;
  }

}
```

### Type: IArtistMap

```cs
public interface IArtistMap : IStatistics, MetaBrainz.Common.Json.IJsonBasedObject {

  System.Collections.Generic.IReadOnlyList<IArtistCountryInfo>? Countries {
    public abstract get;
  }

}
```

### Type: IArtistStatistics

```cs
public interface IArtistStatistics : IStatistics, MetaBrainz.Common.Json.IJsonBasedObject {

  System.Collections.Generic.IReadOnlyList<IArtistInfo>? Artists {
    public abstract get;
  }

  int Count {
    public abstract get;
  }

  int Offset {
    public abstract get;
  }

  int TotalCount {
    public abstract get;
  }

}
```

### Type: IArtistTimeRange

```cs
public interface IArtistTimeRange {

  System.Guid? Id {
    public abstract get;
  }

  int ListenCount {
    public abstract get;
  }

  string Name {
    public abstract get;
  }

  string TimeUnit {
    public abstract get;
  }

}
```

### Type: IDailyActivity

```cs
public interface IDailyActivity : MetaBrainz.Common.Json.IJsonBasedObject {

  System.Collections.Generic.IReadOnlyList<IHourlyActivity>? Friday {
    public abstract get;
  }

  System.Collections.Generic.IReadOnlyList<IHourlyActivity>? Monday {
    public abstract get;
  }

  System.Collections.Generic.IReadOnlyList<IHourlyActivity>? Saturday {
    public abstract get;
  }

  System.Collections.Generic.IReadOnlyList<IHourlyActivity>? Sunday {
    public abstract get;
  }

  System.Collections.Generic.IReadOnlyList<IHourlyActivity>? Thursday {
    public abstract get;
  }

  System.Collections.Generic.IReadOnlyList<IHourlyActivity>? Tuesday {
    public abstract get;
  }

  System.Collections.Generic.IReadOnlyList<IHourlyActivity>? Wednesday {
    public abstract get;
  }

}
```

### Type: IEraActivity

```cs
public interface IEraActivity : IStatistics, MetaBrainz.Common.Json.IJsonBasedObject {

  System.Collections.Generic.IReadOnlyList<IYearlyActivity>? Activity {
    public abstract get;
  }

}
```

### Type: IFetchedListens

```cs
public interface IFetchedListens : MetaBrainz.Common.Json.IJsonBasedObject {

  System.Collections.Generic.IReadOnlyList<IListen> Listens {
    public abstract get;
  }

  System.DateTimeOffset Newest {
    public abstract get;
  }

  System.DateTimeOffset Oldest {
    public abstract get;
  }

  string User {
    public abstract get;
  }

}
```

### Type: IGenreActivity

```cs
public interface IGenreActivity : MetaBrainz.Common.Json.IJsonBasedObject {

  System.Collections.Generic.IReadOnlyList<IGenreActivityDetails>? Activity {
    public abstract get;
  }

}
```

### Type: IGenreActivityDetails

```cs
public interface IGenreActivityDetails : IHourlyActivity, MetaBrainz.Common.Json.IJsonBasedObject {

  string Genre {
    public abstract get;
  }

}
```

### Type: IHourlyActivity

```cs
public interface IHourlyActivity : MetaBrainz.Common.Json.IJsonBasedObject {

  int Hour {
    public abstract get;
  }

  int ListenCount {
    public abstract get;
  }

}
```

### Type: ILatestImport

```cs
public interface ILatestImport : MetaBrainz.Common.Json.IJsonBasedObject {

  System.DateTimeOffset Timestamp {
    public abstract get;
  }

  long UnixTimestamp {
    public abstract get;
  }

  string? User {
    public abstract get;
  }

}
```

### Type: IListen

```cs
public interface IListen : MetaBrainz.Common.Json.IJsonBasedObject {

  System.DateTimeOffset InsertedAt {
    public abstract get;
  }

  System.DateTimeOffset ListenedAt {
    public abstract get;
  }

  System.Guid MessyRecordingId {
    public abstract get;
  }

  ITrackInfo Track {
    public abstract get;
  }

  string User {
    public abstract get;
  }

}
```

### Type: IListenCount

```cs
public interface IListenCount : MetaBrainz.Common.Json.IJsonBasedObject {

  long Count {
    public abstract get;
  }

}
```

### Type: IListenerInfo

```cs
public interface IListenerInfo {

  System.Collections.Generic.IReadOnlyList<ITopListener> TopListeners {
    public abstract get;
  }

  int TotalListeners {
    public abstract get;
  }

  int TotalListens {
    public abstract get;
  }

}
```

### Type: IListeningActivity

```cs
public interface IListeningActivity : IStatistics, MetaBrainz.Common.Json.IJsonBasedObject {

  System.Collections.Generic.IReadOnlyList<IListenTimeRange>? Activity {
    public abstract get;
  }

}
```

### Type: IListenTimeRange

```cs
public interface IListenTimeRange {

  string Description {
    public abstract get;
  }

  int ListenCount {
    public abstract get;
  }

  System.DateTimeOffset? RangeEnd {
    public abstract get;
  }

  System.DateTimeOffset? RangeStart {
    public abstract get;
  }

}
```

### Type: IMusicBrainzIdMappings

```cs
public interface IMusicBrainzIdMappings {

  System.Collections.Generic.IReadOnlyList<System.Guid>? ArtistIds {
    public abstract get;
  }

  long? CoverArtId {
    public abstract get;
  }

  System.Guid? CoverArtReleaseId {
    public abstract get;
  }

  System.Collections.Generic.IReadOnlyList<IArtistCredit>? Credits {
    public abstract get;
  }

  System.Guid? RecordingId {
    public abstract get;
  }

  string? RecordingName {
    public abstract get;
  }

  System.Guid? ReleaseId {
    public abstract get;
  }

}
```

### Type: INewRelease

```cs
public interface INewRelease {

  long? CoverArtId {
    public abstract get;
  }

  System.Guid? CoverArtReleaseId {
    public abstract get;
  }

  string? CreditedArtist {
    public abstract get;
  }

  System.Collections.Generic.IReadOnlyList<System.Guid>? CreditedArtistIds {
    public abstract get;
  }

  System.Collections.Generic.IReadOnlyList<string>? CreditedArtistNames {
    public abstract get;
  }

  System.Collections.Generic.IReadOnlyList<IArtistCredit>? CreditedArtists {
    public abstract get;
  }

  string? FirstReleaseDate {
    public abstract get;
  }

  System.Guid? ReleaseGroupId {
    public abstract get;
  }

  System.Guid? ReleaseId {
    public abstract get;
  }

  string Title {
    public abstract get;
  }

  string? Type {
    public abstract get;
  }

}
```

### Type: IPlayingNow

```cs
public interface IPlayingNow : MetaBrainz.Common.Json.IJsonBasedObject {

  IPlayingTrack? Track {
    public abstract get;
  }

  string User {
    public abstract get;
  }

}
```

### Type: IPlayingTrack

```cs
public interface IPlayingTrack : MetaBrainz.Common.Json.IJsonBasedObject {

  ITrackInfo Info {
    public abstract get;
  }

}
```

### Type: IRecordingInfo

```cs
public interface IRecordingInfo {

  System.Collections.Generic.IReadOnlyList<System.Guid>? ArtistIds {
    public abstract get;
  }

  System.Guid? ArtistMessyId {
    public abstract get;
  }

  string? ArtistName {
    public abstract get;
  }

  long? CoverArtId {
    public abstract get;
  }

  System.Guid? CoverArtReleaseId {
    public abstract get;
  }

  System.Collections.Generic.IReadOnlyList<IArtistCredit>? Credits {
    public abstract get;
  }

  System.Guid? Id {
    public abstract get;
  }

  int ListenCount {
    public abstract get;
  }

  System.Guid? MessyId {
    public abstract get;
  }

  string Name {
    public abstract get;
  }

  System.Guid? ReleaseId {
    public abstract get;
  }

  System.Guid? ReleaseMessyId {
    public abstract get;
  }

  string? ReleaseName {
    public abstract get;
  }

}
```

### Type: IRecordingStatistics

```cs
public interface IRecordingStatistics : IStatistics, MetaBrainz.Common.Json.IJsonBasedObject {

  int? Offset {
    public abstract get;
  }

  System.Collections.Generic.IReadOnlyList<IRecordingInfo>? Recordings {
    public abstract get;
  }

  int? TotalCount {
    public abstract get;
  }

}
```

### Type: IReleaseGroupInfo

```cs
public interface IReleaseGroupInfo : MetaBrainz.Common.Json.IJsonBasedObject {

  System.Collections.Generic.IReadOnlyList<System.Guid>? ArtistIds {
    public abstract get;
  }

  string? ArtistName {
    public abstract get;
  }

  long? CoverArtId {
    public abstract get;
  }

  System.Guid? CoverArtReleaseId {
    public abstract get;
  }

  System.Collections.Generic.IReadOnlyList<IArtistCredit>? Credits {
    public abstract get;
  }

  System.Guid? Id {
    public abstract get;
  }

  int ListenCount {
    public abstract get;
  }

  string Name {
    public abstract get;
  }

}
```

### Type: IReleaseGroupListeners

```cs
public interface IReleaseGroupListeners : IListenerInfo, IStatistics, MetaBrainz.Common.Json.IJsonBasedObject {

  System.Collections.Generic.IReadOnlyList<System.Guid>? ArtistIds {
    public abstract get;
  }

  string? ArtistName {
    public abstract get;
  }

  long? CoverArtId {
    public abstract get;
  }

  System.Guid? CoverArtReleaseId {
    public abstract get;
  }

  System.Guid Id {
    public abstract get;
  }

  string Name {
    public abstract get;
  }

}
```

### Type: IReleaseGroupStatistics

```cs
public interface IReleaseGroupStatistics : IStatistics, MetaBrainz.Common.Json.IJsonBasedObject {

  int? Offset {
    public abstract get;
  }

  System.Collections.Generic.IReadOnlyList<IReleaseGroupInfo>? ReleaseGroups {
    public abstract get;
  }

  int? TotalCount {
    public abstract get;
  }

}
```

### Type: IReleaseInfo

```cs
public interface IReleaseInfo {

  System.Collections.Generic.IReadOnlyList<System.Guid>? ArtistIds {
    public abstract get;
  }

  System.Guid? ArtistMessyId {
    public abstract get;
  }

  string? ArtistName {
    public abstract get;
  }

  long? CoverArtId {
    public abstract get;
  }

  System.Guid? CoverArtReleaseId {
    public abstract get;
  }

  System.Collections.Generic.IReadOnlyList<IArtistCredit>? Credits {
    public abstract get;
  }

  System.Guid? Id {
    public abstract get;
  }

  int ListenCount {
    public abstract get;
  }

  System.Guid? MessyId {
    public abstract get;
  }

  string Name {
    public abstract get;
  }

}
```

### Type: IReleaseStatistics

```cs
public interface IReleaseStatistics : IStatistics, MetaBrainz.Common.Json.IJsonBasedObject {

  int? Offset {
    public abstract get;
  }

  System.Collections.Generic.IReadOnlyList<IReleaseInfo>? Releases {
    public abstract get;
  }

  int? TotalCount {
    public abstract get;
  }

}
```

### Type: ISimilarUser

```cs
public interface ISimilarUser : MetaBrainz.Common.Json.IJsonBasedObject {

  string Name {
    public abstract get;
  }

  decimal Similarity {
    public abstract get;
  }

}
```

### Type: IStatistics

```cs
public interface IStatistics : MetaBrainz.Common.Json.IJsonBasedObject {

  System.DateTimeOffset LastUpdated {
    public abstract get;
  }

  System.DateTimeOffset? NewestListen {
    public abstract get;
  }

  System.DateTimeOffset? OldestListen {
    public abstract get;
  }

  MetaBrainz.ListenBrainz.StatisticsRange Range {
    public abstract get;
  }

  string? User {
    public abstract get;
  }

}
```

### Type: ISubmittedListen

```cs
public interface ISubmittedListen : ISubmittedListenData {

  System.DateTimeOffset Timestamp {
    public abstract get;
  }

  long UnixTimestamp {
    public abstract get;
  }

}
```

### Type: ISubmittedListenData

```cs
public interface ISubmittedListenData {

  ISubmittedTrackInfo Track {
    public abstract get;
  }

}
```

### Type: ISubmittedTrackInfo

```cs
public interface ISubmittedTrackInfo {

  System.Collections.Generic.IReadOnlyDictionary<string, object?>? AdditionalInfo {
    public abstract get;
  }

  string Artist {
    public abstract get;
  }

  string Name {
    public abstract get;
  }

  string? Release {
    public abstract get;
  }

}
```

### Type: ITokenValidationResult

```cs
public interface ITokenValidationResult {

  System.Net.HttpStatusCode Code {
    public abstract get;
  }

  string Message {
    public abstract get;
  }

  string? User {
    public abstract get;
  }

  bool? Valid {
    public abstract get;
  }

}
```

### Type: ITopArtist

```cs
public interface ITopArtist : MetaBrainz.Common.Json.IJsonBasedObject {

  System.Guid? Id {
    public abstract get;
  }

  int ListenCount {
    public abstract get;
  }

  string Name {
    public abstract get;
  }

}
```

### Type: ITopGenre

```cs
public interface ITopGenre : MetaBrainz.Common.Json.IJsonBasedObject {

  string Genre {
    public abstract get;
  }

  int ListenCount {
    public abstract get;
  }

  decimal Percentage {
    public abstract get;
  }

}
```

### Type: ITopListener

```cs
public interface ITopListener : MetaBrainz.Common.Json.IJsonBasedObject {

  int ListenCount {
    public abstract get;
  }

  string UserName {
    public abstract get;
  }

}
```

### Type: ITopRecording

```cs
public interface ITopRecording : MetaBrainz.Common.Json.IJsonBasedObject {

  System.Collections.Generic.IReadOnlyList<System.Guid>? ArtistIds {
    public abstract get;
  }

  string? ArtistName {
    public abstract get;
  }

  long? CoverArtId {
    public abstract get;
  }

  System.Guid? CoverArtReleaseId {
    public abstract get;
  }

  System.Collections.Generic.IReadOnlyList<IArtistCredit>? Credits {
    public abstract get;
  }

  System.Guid? Id {
    public abstract get;
  }

  int ListenCount {
    public abstract get;
  }

  string Name {
    public abstract get;
  }

  System.Guid? ReleaseId {
    public abstract get;
  }

  string? ReleaseName {
    public abstract get;
  }

}
```

### Type: ITopRelease

```cs
public interface ITopRelease : MetaBrainz.Common.Json.IJsonBasedObject {

  System.Collections.Generic.IReadOnlyList<System.Guid>? ArtistIds {
    public abstract get;
  }

  string? ArtistName {
    public abstract get;
  }

  long? CoverArtId {
    public abstract get;
  }

  System.Guid? CoverArtReleaseId {
    public abstract get;
  }

  System.Guid? Id {
    public abstract get;
  }

  int ListenCount {
    public abstract get;
  }

  string Name {
    public abstract get;
  }

}
```

### Type: ITrackInfo

```cs
public interface ITrackInfo : MetaBrainz.Common.Json.IJsonBasedObject {

  IAdditionalInfo AdditionalInfo {
    public abstract get;
  }

  string Artist {
    public abstract get;
  }

  IMusicBrainzIdMappings? MusicBrainzIdMappings {
    public abstract get;
  }

  string Name {
    public abstract get;
  }

  string? Release {
    public abstract get;
  }

}
```

### Type: IUserDailyActivity

```cs
public interface IUserDailyActivity : IStatistics, MetaBrainz.Common.Json.IJsonBasedObject {

  IDailyActivity? Activity {
    public abstract get;
  }

}
```

### Type: IYearInMusic

```cs
public interface IYearInMusic : MetaBrainz.Common.Json.IJsonBasedObject {

  IYearInMusicData Data {
    public abstract get;
  }

  string User {
    public abstract get;
  }

}
```

### Type: IYearInMusicData

```cs
public interface IYearInMusicData : MetaBrainz.Common.Json.IJsonBasedObject {

  int? ArtistCount {
    public abstract get;
  }

  System.Collections.Generic.IReadOnlyList<IArtistCountryInfo>? ArtistMap {
    public abstract get;
  }

  string? DayOfWeek {
    public abstract get;
  }

  int? ListenCount {
    public abstract get;
  }

  System.TimeSpan? ListeningTime {
    public abstract get;
  }

  System.Collections.Generic.IReadOnlyList<IListenTimeRange>? ListensPerDay {
    public abstract get;
  }

  System.Collections.Generic.IReadOnlyDictionary<string, int>? MostListenedYear {
    public abstract get;
  }

  string? MostProminentColor {
    public abstract get;
  }

  int? NewArtistsDiscovered {
    public abstract get;
  }

  System.Collections.Generic.IReadOnlyList<INewRelease>? NewReleasesOfTopArtists {
    public abstract get;
  }

  int? RecordingCount {
    public abstract get;
  }

  int? ReleaseCount {
    public abstract get;
  }

  int? ReleaseGroupCount {
    public abstract get;
  }

  System.Collections.Generic.IReadOnlyDictionary<string, decimal>? SimilarUsers {
    public abstract get;
  }

  System.Collections.Generic.IReadOnlyList<ITopArtist>? TopArtists {
    public abstract get;
  }

  MetaBrainz.ListenBrainz.Interfaces.JSPF.IPlaylist? TopDiscoveriesPlaylist {
    public abstract get;
  }

  System.Collections.Generic.IReadOnlyDictionary<string, System.Uri>? TopDiscoveriesPlaylistCoverArt {
    public abstract get;
  }

  System.Collections.Generic.IReadOnlyList<ITopGenre>? TopGenres {
    public abstract get;
  }

  MetaBrainz.ListenBrainz.Interfaces.JSPF.IPlaylist? TopMissedRecordingsPlaylist {
    public abstract get;
  }

  System.Collections.Generic.IReadOnlyDictionary<string, System.Uri>? TopMissedRecordingsPlaylistCoverArt {
    public abstract get;
  }

  MetaBrainz.ListenBrainz.Interfaces.JSPF.IPlaylist? TopNewRecordingsPlaylist {
    public abstract get;
  }

  System.Collections.Generic.IReadOnlyDictionary<string, System.Uri>? TopNewRecordingsPlaylistCoverArt {
    public abstract get;
  }

  System.Collections.Generic.IReadOnlyList<ITopRecording>? TopRecordings {
    public abstract get;
  }

  MetaBrainz.ListenBrainz.Interfaces.JSPF.IPlaylist? TopRecordingsPlaylist {
    public abstract get;
  }

  System.Collections.Generic.IReadOnlyDictionary<string, System.Uri>? TopRecordingsPlaylistCoverArt {
    public abstract get;
  }

  System.Collections.Generic.IReadOnlyList<IReleaseGroupInfo>? TopReleaseGroups {
    public abstract get;
  }

  System.Collections.Generic.IReadOnlyList<ITopRelease>? TopReleases {
    public abstract get;
  }

  System.Collections.Generic.IReadOnlyDictionary<string, System.Uri>? TopReleasesCoverArt {
    public abstract get;
  }

}
```

### Type: IYearlyActivity

```cs
public interface IYearlyActivity : MetaBrainz.Common.Json.IJsonBasedObject {

  int ListenCount {
    public abstract get;
  }

  int Year {
    public abstract get;
  }

}
```

## Namespace: MetaBrainz.ListenBrainz.Interfaces.JSPF

### Type: ILink

```cs
public interface ILink {

  System.Uri Id {
    public abstract get;
  }

  System.Uri Value {
    public abstract get;
  }

}
```

### Type: IMeta

```cs
public interface IMeta {

  System.Uri Id {
    public abstract get;
  }

  string Value {
    public abstract get;
  }

}
```

### Type: IMusicBrainzPlaylist

```cs
public interface IMusicBrainzPlaylist {

  System.Collections.Generic.IReadOnlyDictionary<string, object?>? AdditionalMetadata {
    public abstract get;
  }

  System.Collections.Generic.IReadOnlyList<string>? Collaborators {
    public abstract get;
  }

  System.Uri? CopiedFrom {
    public abstract get;
  }

  bool? CopiedFromDeleted {
    public abstract get;
  }

  string? CreatedFor {
    public abstract get;
  }

  string? Creator {
    public abstract get;
  }

  System.DateTimeOffset? LastModified {
    public abstract get;
  }

  bool? Public {
    public abstract get;
  }

}
```

### Type: IMusicBrainzRecording

```cs
public interface IMusicBrainzRecording {

  System.Collections.Generic.IReadOnlyList<System.Guid>? ArtistIds {
    public abstract get;
  }

}
```

### Type: IMusicBrainzTrack

```cs
public interface IMusicBrainzTrack : MetaBrainz.Common.Json.IJsonBasedObject {

  System.DateTimeOffset? Added {
    public abstract get;
  }

  string? AddedBy {
    public abstract get;
  }

  System.Collections.Generic.IReadOnlyDictionary<string, object?>? AdditionalMetadata {
    public abstract get;
  }

  System.Collections.Generic.IReadOnlyList<System.Uri>? ArtistIds {
    public abstract get;
  }

  long? CoverArtId {
    public abstract get;
  }

  System.Guid? CoverArtReleaseId {
    public abstract get;
  }

  System.Collections.Generic.IReadOnlyList<MetaBrainz.ListenBrainz.Interfaces.IArtistCredit>? Credits {
    public abstract get;
  }

  System.Uri? ReleaseId {
    public abstract get;
  }

}
```

### Type: INamedUri

```cs
public interface INamedUri {

  string Name {
    public abstract get;
  }

  System.Uri Uri {
    public abstract get;
  }

}
```

### Type: IPlaylist

```cs
public interface IPlaylist : MetaBrainz.Common.Json.IJsonBasedObject {

  string? Annotation {
    public abstract get;
  }

  System.Collections.Generic.IReadOnlyList<INamedUri>? Attribution {
    public abstract get;
  }

  string? Creator {
    public abstract get;
  }

  System.DateTimeOffset? Date {
    public abstract get;
  }

  System.Collections.Generic.IReadOnlyDictionary<System.Uri, System.Collections.Generic.IReadOnlyList<object?>?>? Extensions {
    public abstract get;
  }

  System.Uri? Identifier {
    public abstract get;
  }

  System.Uri? Image {
    public abstract get;
  }

  System.Uri? Info {
    public abstract get;
  }

  System.Uri? License {
    public abstract get;
  }

  System.Collections.Generic.IReadOnlyList<ILink>? Links {
    public abstract get;
  }

  System.Uri? Location {
    public abstract get;
  }

  System.Collections.Generic.IReadOnlyList<IMeta>? Metadata {
    public abstract get;
  }

  IMusicBrainzPlaylist? MusicBrainz {
    public abstract get;
  }

  string? Title {
    public abstract get;
  }

  System.Collections.Generic.IReadOnlyList<ITrack> Tracks {
    public abstract get;
  }

}
```

### Type: ITrack

```cs
public interface ITrack : MetaBrainz.Common.Json.IJsonBasedObject {

  string? Album {
    public abstract get;
  }

  string? Annotation {
    public abstract get;
  }

  string? Creator {
    public abstract get;
  }

  System.TimeSpan? Duration {
    public abstract get;
  }

  System.Collections.Generic.IReadOnlyDictionary<System.Uri, System.Collections.Generic.IReadOnlyList<object?>?>? Extensions {
    public abstract get;
  }

  System.Collections.Generic.IReadOnlyList<System.Uri>? Identifiers {
    public abstract get;
  }

  System.Uri? Image {
    public abstract get;
  }

  System.Uri? Info {
    public abstract get;
  }

  System.Collections.Generic.IReadOnlyList<ILink>? Links {
    public abstract get;
  }

  System.Collections.Generic.IReadOnlyList<System.Uri>? Locations {
    public abstract get;
  }

  System.Collections.Generic.IReadOnlyList<IMeta>? Metadata {
    public abstract get;
  }

  IMusicBrainzTrack? MusicBrainz {
    public abstract get;
  }

  IMusicBrainzRecording? MusicBrainzRecording {
    public abstract get;
  }

  string? Title {
    public abstract get;
  }

  uint? TrackNumber {
    public abstract get;
  }

}
```

## Namespace: MetaBrainz.ListenBrainz.Objects

### Type: SubmittedListen

```cs
public class SubmittedListen : SubmittedListenData, MetaBrainz.ListenBrainz.Interfaces.ISubmittedListen, MetaBrainz.ListenBrainz.Interfaces.ISubmittedListenData {

  System.DateTimeOffset Timestamp {
    public sealed override get;
    public set;
  }

  long UnixTimestamp {
    public sealed override get;
    public set;
  }

  public SubmittedListen();

  [System.Diagnostics.CodeAnalysis.SetsRequiredMembersAttribute]
  [System.ObsoleteAttribute("Use an object initializer to set the properties.")]
  public SubmittedListen(System.DateTimeOffset timestamp, string track, string artist, string? release = null);

  [System.Diagnostics.CodeAnalysis.SetsRequiredMembersAttribute]
  [System.ObsoleteAttribute("Use an object initializer to set the properties.")]
  public SubmittedListen(long timestamp, string track, string artist, string? release = null);

  [System.Diagnostics.CodeAnalysis.SetsRequiredMembersAttribute]
  [System.ObsoleteAttribute("Use an object initializer to set the properties.")]
  public SubmittedListen(string track, string artist, string? release = null);

}
```

### Type: SubmittedListenData

```cs
public class SubmittedListenData : MetaBrainz.ListenBrainz.Interfaces.ISubmittedListenData {

  required MetaBrainz.ListenBrainz.Interfaces.ISubmittedTrackInfo Track {
    public sealed override get;
    public init;
  }

  public SubmittedListenData();

  [System.Diagnostics.CodeAnalysis.SetsRequiredMembersAttribute]
  [System.ObsoleteAttribute("Use an object initializer to set the track info.")]
  public SubmittedListenData(string track, string artist, string? release = null);

}
```

### Type: SubmittedTrackInfo

```cs
public class SubmittedTrackInfo : MetaBrainz.ListenBrainz.Interfaces.ISubmittedTrackInfo {

  System.Collections.Generic.Dictionary<string, object?>? AdditionalInfo {
    public get;
    public set;
  }

  required string Artist {
    public sealed override get;
    public set;
  }

  required string Name {
    public sealed override get;
    public set;
  }

  string? Release {
    public sealed override get;
    public set;
  }

  public SubmittedTrackInfo();

  [System.Diagnostics.CodeAnalysis.SetsRequiredMembersAttribute]
  [System.ObsoleteAttribute("Use an object initializer to set the properties.")]
  public SubmittedTrackInfo(string name, string artist, string? release = null);

}
```
