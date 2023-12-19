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

  public const int MaxItemsPerGet = 100;

  public const int MaxListenSize = 10240;

  public const int MaxTagLength = 64;

  public const int MaxTagsPerListen = 50;

  public const int MaxTimeRange = 73;

  public const string UserAgentUrl = "https://github.com/Zastai/ListenBrainz";

  public const string WebServiceRoot = "/1/";

  System.Uri BaseUri {
    public get;
  }

  System.Uri ContactInfo {
    public get;
  }

  System.Uri? DefaultContactInfo {
    public static get;
    public static set;
  }

  int DefaultPort {
    public static get;
    public static set;
  }

  System.Net.Http.Headers.ProductHeaderValue? DefaultProductInfo {
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

  string? DefaultUserToken {
    public static get;
    public static set;
  }

  int Port {
    public get;
    public set;
  }

  System.Net.Http.Headers.ProductHeaderValue ProductInfo {
    public get;
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

  string? UserToken {
    public get;
    public set;
  }

  public ListenBrainz();

  public ListenBrainz(System.Net.Http.Headers.ProductHeaderValue product);

  public ListenBrainz(System.Net.Http.Headers.ProductHeaderValue product, System.Uri contact);

  public ListenBrainz(System.Net.Http.Headers.ProductHeaderValue product, string contact);

  public ListenBrainz(System.Uri contact);

  public ListenBrainz(string contact);

  public ListenBrainz(string application, System.Version version);

  public ListenBrainz(string application, System.Version version, System.Uri contact);

  public ListenBrainz(string application, System.Version version, string contact);

  public ListenBrainz(string application, string version);

  public ListenBrainz(string application, string version, System.Uri contact);

  public ListenBrainz(string application, string version, string contact);

  public void Close();

  public sealed override void Dispose();

  protected override void Finalize();

  public MetaBrainz.ListenBrainz.Interfaces.IUserArtistMap? GetArtistMap(string user, StatisticsRange? range = default, bool forceRecalculation = false);

  public System.Threading.Tasks.Task<MetaBrainz.ListenBrainz.Interfaces.IUserArtistMap?> GetArtistMapAsync(string user, StatisticsRange? range = default, bool forceRecalculation = false, System.Threading.CancellationToken cancellationToken = default);

  public MetaBrainz.ListenBrainz.Interfaces.ISiteArtistStatistics? GetArtistStatistics(int? count = default, int? offset = default, StatisticsRange? range = default);

  public MetaBrainz.ListenBrainz.Interfaces.IUserArtistStatistics? GetArtistStatistics(string user, int? count = default, int? offset = default, StatisticsRange? range = default);

  public System.Threading.Tasks.Task<MetaBrainz.ListenBrainz.Interfaces.ISiteArtistStatistics?> GetArtistStatisticsAsync(int? count = default, int? offset = default, StatisticsRange? range = default, System.Threading.CancellationToken cancellationToken = default);

  public System.Threading.Tasks.Task<MetaBrainz.ListenBrainz.Interfaces.IUserArtistStatistics?> GetArtistStatisticsAsync(string user, int? count = default, int? offset = default, StatisticsRange? range = default, System.Threading.CancellationToken cancellationToken = default);

  public MetaBrainz.ListenBrainz.Interfaces.IUserDailyActivity? GetDailyActivity(string user, StatisticsRange? range = default);

  public System.Threading.Tasks.Task<MetaBrainz.ListenBrainz.Interfaces.IUserDailyActivity?> GetDailyActivityAsync(string user, StatisticsRange? range = default, System.Threading.CancellationToken cancellationToken = default);

  public MetaBrainz.ListenBrainz.Interfaces.ILatestImport GetLatestImport(string user);

  public System.Threading.Tasks.Task<MetaBrainz.ListenBrainz.Interfaces.ILatestImport> GetLatestImportAsync(string user, System.Threading.CancellationToken cancellationToken = default);

  public MetaBrainz.ListenBrainz.Interfaces.IListenCount GetListenCount(string user);

  public System.Threading.Tasks.Task<MetaBrainz.ListenBrainz.Interfaces.IListenCount> GetListenCountAsync(string user, System.Threading.CancellationToken cancellationToken = default);

  public MetaBrainz.ListenBrainz.Interfaces.IUserListeningActivity? GetListeningActivity(string user, StatisticsRange? range = default);

  public System.Threading.Tasks.Task<MetaBrainz.ListenBrainz.Interfaces.IUserListeningActivity?> GetListeningActivityAsync(string user, StatisticsRange? range = default, System.Threading.CancellationToken cancellationToken = default);

  public MetaBrainz.ListenBrainz.Interfaces.IFetchedListens GetListens(string user, int? count = default, int? timeRange = default);

  public MetaBrainz.ListenBrainz.Interfaces.IFetchedListens GetListensAfter(string user, System.DateTimeOffset after, int? count = default, int? timeRange = default);

  public MetaBrainz.ListenBrainz.Interfaces.IFetchedListens GetListensAfter(string user, long after, int? count = default, int? timeRange = default);

  public System.Threading.Tasks.Task<MetaBrainz.ListenBrainz.Interfaces.IFetchedListens> GetListensAfterAsync(string user, System.DateTimeOffset after, int? count = default, int? timeRange = default, System.Threading.CancellationToken cancellationToken = default);

  public System.Threading.Tasks.Task<MetaBrainz.ListenBrainz.Interfaces.IFetchedListens> GetListensAfterAsync(string user, long after, int? count = default, int? timeRange = default, System.Threading.CancellationToken cancellationToken = default);

  public System.Threading.Tasks.Task<MetaBrainz.ListenBrainz.Interfaces.IFetchedListens> GetListensAsync(string user, int? count = default, int? timeRange = default, System.Threading.CancellationToken cancellationToken = default);

  public MetaBrainz.ListenBrainz.Interfaces.IFetchedListens GetListensBefore(string user, System.DateTimeOffset before, int? count = default, int? timeRange = default);

  public MetaBrainz.ListenBrainz.Interfaces.IFetchedListens GetListensBefore(string user, long before, int? count = default, int? timeRange = default);

  public System.Threading.Tasks.Task<MetaBrainz.ListenBrainz.Interfaces.IFetchedListens> GetListensBeforeAsync(string user, System.DateTimeOffset before, int? count = default, int? timeRange = default, System.Threading.CancellationToken cancellationToken = default);

  public System.Threading.Tasks.Task<MetaBrainz.ListenBrainz.Interfaces.IFetchedListens> GetListensBeforeAsync(string user, long before, int? count = default, int? timeRange = default, System.Threading.CancellationToken cancellationToken = default);

  public MetaBrainz.ListenBrainz.Interfaces.IFetchedListens GetListensBetween(string user, System.DateTimeOffset after, System.DateTimeOffset before, int? count = default);

  public MetaBrainz.ListenBrainz.Interfaces.IFetchedListens GetListensBetween(string user, long after, long before, int? count = default);

  public System.Threading.Tasks.Task<MetaBrainz.ListenBrainz.Interfaces.IFetchedListens> GetListensBetweenAsync(string user, System.DateTimeOffset after, System.DateTimeOffset before, int? count = default, System.Threading.CancellationToken cancellationToken = default);

  public System.Threading.Tasks.Task<MetaBrainz.ListenBrainz.Interfaces.IFetchedListens> GetListensBetweenAsync(string user, long after, long before, int? count = default, System.Threading.CancellationToken cancellationToken = default);

  public MetaBrainz.ListenBrainz.Interfaces.IPlayingNow GetPlayingNow(string user);

  public System.Threading.Tasks.Task<MetaBrainz.ListenBrainz.Interfaces.IPlayingNow> GetPlayingNowAsync(string user, System.Threading.CancellationToken cancellationToken = default);

  public MetaBrainz.ListenBrainz.Interfaces.IRecentListens GetRecentListens(System.Collections.Generic.IEnumerable<string> users);

  public MetaBrainz.ListenBrainz.Interfaces.IRecentListens GetRecentListens(int limit, System.Collections.Generic.IEnumerable<string> users);

  public MetaBrainz.ListenBrainz.Interfaces.IRecentListens GetRecentListens(int limit, params string[] users);

  public MetaBrainz.ListenBrainz.Interfaces.IRecentListens GetRecentListens(params string[] users);

  public System.Threading.Tasks.Task<MetaBrainz.ListenBrainz.Interfaces.IRecentListens> GetRecentListensAsync(System.Collections.Generic.IEnumerable<string> users, System.Threading.CancellationToken cancellationToken = default);

  public System.Threading.Tasks.Task<MetaBrainz.ListenBrainz.Interfaces.IRecentListens> GetRecentListensAsync(System.Threading.CancellationToken cancellationToken, params string[] users);

  public System.Threading.Tasks.Task<MetaBrainz.ListenBrainz.Interfaces.IRecentListens> GetRecentListensAsync(int limit, System.Collections.Generic.IEnumerable<string> users, System.Threading.CancellationToken cancellationToken = default);

  public System.Threading.Tasks.Task<MetaBrainz.ListenBrainz.Interfaces.IRecentListens> GetRecentListensAsync(int limit, System.Threading.CancellationToken cancellationToken, params string[] users);

  public System.Threading.Tasks.Task<MetaBrainz.ListenBrainz.Interfaces.IRecentListens> GetRecentListensAsync(int limit, params string[] users);

  public System.Threading.Tasks.Task<MetaBrainz.ListenBrainz.Interfaces.IRecentListens> GetRecentListensAsync(params string[] users);

  public MetaBrainz.ListenBrainz.Interfaces.IUserRecordingStatistics? GetRecordingStatistics(string user, int? count = default, int? offset = default, StatisticsRange? range = default);

  public System.Threading.Tasks.Task<MetaBrainz.ListenBrainz.Interfaces.IUserRecordingStatistics?> GetRecordingStatisticsAsync(string user, int? count = default, int? offset = default, StatisticsRange? range = default, System.Threading.CancellationToken cancellationToken = default);

  public MetaBrainz.ListenBrainz.Interfaces.IUserReleaseStatistics? GetReleaseStatistics(string user, int? count = default, int? offset = default, StatisticsRange? range = default);

  public System.Threading.Tasks.Task<MetaBrainz.ListenBrainz.Interfaces.IUserReleaseStatistics?> GetReleaseStatisticsAsync(string user, int? count = default, int? offset = default, StatisticsRange? range = default, System.Threading.CancellationToken cancellationToken = default);

  public void ImportListens(params MetaBrainz.ListenBrainz.Interfaces.ISubmittedListen[] listens);

  public void ImportListens(System.Collections.Generic.IEnumerable<MetaBrainz.ListenBrainz.Interfaces.ISubmittedListen> listens);

  public System.Threading.Tasks.Task ImportListensAsync(params MetaBrainz.ListenBrainz.Interfaces.ISubmittedListen[] listens);

  public System.Threading.Tasks.Task ImportListensAsync(System.Collections.Generic.IAsyncEnumerable<MetaBrainz.ListenBrainz.Interfaces.ISubmittedListen> listens, System.Threading.CancellationToken cancellationToken = default);

  public System.Threading.Tasks.Task ImportListensAsync(System.Collections.Generic.IEnumerable<MetaBrainz.ListenBrainz.Interfaces.ISubmittedListen> listens, System.Threading.CancellationToken cancellationToken = default);

  public System.Threading.Tasks.Task ImportListensAsync(System.Threading.CancellationToken cancellationToken, params MetaBrainz.ListenBrainz.Interfaces.ISubmittedListen[] listens);

  public void SetLatestImport(string user, System.DateTimeOffset timestamp);

  public void SetLatestImport(string user, long timestamp);

  public System.Threading.Tasks.Task SetLatestImportAsync(string user, System.DateTimeOffset timestamp, System.Threading.CancellationToken cancellationToken = default);

  public System.Threading.Tasks.Task SetLatestImportAsync(string user, long timestamp, System.Threading.CancellationToken cancellationToken = default);

  public void SetNowPlaying(MetaBrainz.ListenBrainz.Interfaces.ISubmittedListenData listen);

  public void SetNowPlaying(string track, string artist, string? release = null);

  public System.Threading.Tasks.Task SetNowPlayingAsync(MetaBrainz.ListenBrainz.Interfaces.ISubmittedListenData listen, System.Threading.CancellationToken cancellationToken = default);

  public System.Threading.Tasks.Task SetNowPlayingAsync(string track, string artist, string? release = null, System.Threading.CancellationToken cancellationToken = default);

  public void SubmitSingleListen(MetaBrainz.ListenBrainz.Interfaces.ISubmittedListen listen);

  public void SubmitSingleListen(System.DateTimeOffset timestamp, string track, string artist, string? release = null);

  public void SubmitSingleListen(long timestamp, string track, string artist, string? release = null);

  public void SubmitSingleListen(string track, string artist, string? release = null);

  public System.Threading.Tasks.Task SubmitSingleListenAsync(MetaBrainz.ListenBrainz.Interfaces.ISubmittedListen listen, System.Threading.CancellationToken cancellationToken = default);

  public System.Threading.Tasks.Task SubmitSingleListenAsync(System.DateTimeOffset timestamp, string track, string artist, string? release = null, System.Threading.CancellationToken cancellationToken = default);

  public System.Threading.Tasks.Task SubmitSingleListenAsync(long timestamp, string track, string artist, string? release = null, System.Threading.CancellationToken cancellationToken = default);

  public System.Threading.Tasks.Task SubmitSingleListenAsync(string track, string artist, string? release = null, System.Threading.CancellationToken cancellationToken = default);

  public MetaBrainz.ListenBrainz.Interfaces.ITokenValidationResult ValidateToken(string token);

  public System.Threading.Tasks.Task<MetaBrainz.ListenBrainz.Interfaces.ITokenValidationResult> ValidateTokenAsync(string token, System.Threading.CancellationToken cancellationToken = default);

}
```

### Type: StatisticsRange

```cs
public enum StatisticsRange {

  AllTime = 0,
  Month = 2,
  Unknown = 4,
  Week = 1,
  Year = 3,

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

### Type: IArtistCountryInfo

```cs
public interface IArtistCountryInfo {

  int ArtistCount {
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

### Type: IArtistInfo

```cs
public interface IArtistInfo : MetaBrainz.Common.Json.IJsonBasedObject {

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

### Type: IArtistStatistics

```cs
public interface IArtistStatistics {

  System.Collections.Generic.IReadOnlyList<IArtistInfo>? Artists {
    public abstract get;
  }

  int Count {
    public abstract get;
  }

  int Offset {
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

### Type: IFetchedListens

```cs
public interface IFetchedListens : MetaBrainz.Common.Json.IJsonBasedObject {

  System.Collections.Generic.IReadOnlyList<IListen> Listens {
    public abstract get;
  }

  System.DateTimeOffset Timestamp {
    public abstract get;
  }

  long UnixTimestamp {
    public abstract get;
  }

  string User {
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

  System.DateTimeOffset? Timestamp {
    public abstract get;
  }

  long? UnixTimestamp {
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

  string InsertedAt {
    public abstract get;
  }

  System.Guid MessyRecordingId {
    public abstract get;
  }

  System.DateTimeOffset Timestamp {
    public abstract get;
  }

  ITrackInfo Track {
    public abstract get;
  }

  long UnixTimestamp {
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

  System.Guid? RecordingId {
    public abstract get;
  }

  System.Guid? ReleaseId {
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

### Type: IRecentListens

```cs
public interface IRecentListens : MetaBrainz.Common.Json.IJsonBasedObject {

  System.Collections.Generic.IReadOnlyList<IListen> Listens {
    public abstract get;
  }

  string UserList {
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

### Type: ISiteArtistStatistics

```cs
public interface ISiteArtistStatistics : IArtistStatistics, IStatistics, MetaBrainz.Common.Json.IJsonBasedObject {

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

}
```

### Type: ISubmittedListen

```cs
public interface ISubmittedListen : ISubmittedListenData {

  System.DateTimeOffset Timestamp {
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

### Type: IUserArtistMap

```cs
public interface IUserArtistMap : IStatistics, IUserStatistics, MetaBrainz.Common.Json.IJsonBasedObject {

  System.Collections.Generic.IReadOnlyList<IArtistCountryInfo>? Countries {
    public abstract get;
  }

}
```

### Type: IUserArtistStatistics

```cs
public interface IUserArtistStatistics : IArtistStatistics, IStatistics, IUserStatistics, MetaBrainz.Common.Json.IJsonBasedObject {

  int TotalCount {
    public abstract get;
  }

}
```

### Type: IUserDailyActivity

```cs
public interface IUserDailyActivity : IStatistics, IUserStatistics, MetaBrainz.Common.Json.IJsonBasedObject {

  IDailyActivity? Activity {
    public abstract get;
  }

}
```

### Type: IUserListeningActivity

```cs
public interface IUserListeningActivity : IStatistics, IUserStatistics, MetaBrainz.Common.Json.IJsonBasedObject {

  System.Collections.Generic.IReadOnlyList<IListenTimeRange>? Activity {
    public abstract get;
  }

}
```

### Type: IUserRecordingStatistics

```cs
public interface IUserRecordingStatistics : IStatistics, IUserStatistics, MetaBrainz.Common.Json.IJsonBasedObject {

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

### Type: IUserReleaseStatistics

```cs
public interface IUserReleaseStatistics : IStatistics, IUserStatistics, MetaBrainz.Common.Json.IJsonBasedObject {

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

### Type: IUserStatistics

```cs
public interface IUserStatistics : IStatistics, MetaBrainz.Common.Json.IJsonBasedObject {

  string User {
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

  public SubmittedListen(System.DateTimeOffset timestamp, string track, string artist, string? release = null);

  public SubmittedListen(long timestamp, string track, string artist, string? release = null);

  public SubmittedListen(string track, string artist, string? release = null);

}
```

### Type: SubmittedListenData

```cs
public class SubmittedListenData : MetaBrainz.ListenBrainz.Interfaces.ISubmittedListenData {

  public readonly SubmittedTrackInfo Track;

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

  string Artist {
    public sealed override get;
    public set;
  }

  string Name {
    public sealed override get;
    public set;
  }

  string? Release {
    public sealed override get;
    public set;
  }

  public SubmittedTrackInfo(string name, string artist, string? release = null);

}
```
