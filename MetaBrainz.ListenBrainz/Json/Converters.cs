using System.Collections.Generic;
using System.Text.Json.Serialization;

using JetBrains.Annotations;

using MetaBrainz.Common.Json.Converters;
using MetaBrainz.ListenBrainz.Json.Readers;
using MetaBrainz.ListenBrainz.Json.Writers;

namespace MetaBrainz.ListenBrainz.Json {

  internal static class Converters {

    public static IEnumerable<JsonConverter> Readers {
      get {
        yield return ErrorInfoReader.Instance;
        yield return FetchedListensReader.Instance;
        yield return LatestImportReader.Instance;
        yield return PlayingNowReader.Instance;
        yield return RecentListensReader.Instance;
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
