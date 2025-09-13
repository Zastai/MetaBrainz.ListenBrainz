using System.Collections.Generic;
using System.Text.Json.Serialization;

using MetaBrainz.ListenBrainz.Json.Readers;
using MetaBrainz.ListenBrainz.Json.Writers;

namespace MetaBrainz.ListenBrainz.Json;

internal static class Converters {

  public static IEnumerable<JsonConverter> Readers {
    get {
      yield return ArtistActivityReader.Instance;
      yield return ArtistEvolutionActivityReader.Instance;
      yield return ArtistListenersReader.Instance;
      yield return ErrorInfoReader.Instance;
      yield return FetchedListensReader.Instance;
      yield return LatestImportReader.Instance;
      yield return ListenCountReader.Instance;
      yield return PlayingNowReader.Instance;
      yield return ReleaseGroupListenersReader.Instance;
      yield return SiteArtistMapReader.Instance;
      yield return SiteArtistStatisticsReader.Instance;
      yield return SiteListeningActivityReader.Instance;
      yield return SiteRecordingStatisticsReader.Instance;
      yield return SiteReleaseGroupStatisticsReader.Instance;
      yield return SiteReleaseStatisticsReader.Instance;
      yield return TokenValidationResultReader.Instance;
      yield return UserArtistMapReader.Instance;
      yield return UserArtistStatisticsReader.Instance;
      yield return UserDailyActivityReader.Instance;
      yield return UserListeningActivityReader.Instance;
      yield return UserRecordingStatisticsReader.Instance;
      yield return UserReleaseGroupStatisticsReader.Instance;
      yield return UserReleaseStatisticsReader.Instance;
    }
  }

  public static IEnumerable<JsonConverter> Writers {
    get {
      yield return ListenPayloadWriter.Instance;
      yield return ListenDataPayloadWriter.Instance;
    }
  }

}
