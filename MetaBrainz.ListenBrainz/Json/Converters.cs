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
      yield return ArtistMapReader.Instance;
      yield return ArtistStatisticsReader.Instance;
      yield return EraActivityReader.Instance;
      yield return ErrorInfoReader.Instance;
      yield return FetchedListensReader.Instance;
      yield return GenreActivityReader.Instance;
      yield return LatestImportReader.Instance;
      yield return ListenCountReader.Instance;
      yield return ListeningActivityReader.Instance;
      yield return PlayingNowReader.Instance;
      yield return RecordingStatisticsReader.Instance;
      yield return ReleaseGroupListenersReader.Instance;
      yield return ReleaseGroupStatisticsReader.Instance;
      yield return ReleaseStatisticsReader.Instance;
      yield return TokenValidationResultReader.Instance;
      yield return UserDailyActivityReader.Instance;
    }
  }

  public static IEnumerable<JsonConverter> Writers {
    get {
      yield return ListenPayloadWriter.Instance;
      yield return ListenDataPayloadWriter.Instance;
    }
  }

}
