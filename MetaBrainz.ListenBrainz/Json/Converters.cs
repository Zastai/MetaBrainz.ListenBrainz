using System.Collections.Generic;
using System.Text.Json.Serialization;

using MetaBrainz.ListenBrainz.Json.Readers;
using MetaBrainz.ListenBrainz.Json.Writers;

namespace MetaBrainz.ListenBrainz.Json {

  internal static class Converters {

    public static IEnumerable<JsonConverter> Readers {
      get {
        yield return ErrorInfoReader.Instance;
        yield return FetchedListensReader.Instance;
        yield return LatestImportReader.Instance;
        yield return ListenCountReader.Instance;
        yield return PlayingNowReader.Instance;
        yield return RecentListensReader.Instance;
        yield return TokenValidationResultReader.Instance;
        yield return UserArtistStatisticsReader.Instance;
        yield return UserRecordingStatisticsReader.Instance;
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

}
