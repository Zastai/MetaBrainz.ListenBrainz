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
        // Readers for specific (top-level) types
        yield return ErrorInfoReader.Instance;
        yield return FetchedListensReader.Instance;
        yield return LatestImportReader.Instance;
        yield return PlayingNowReader.Instance;
        yield return RecentListensReader.Instance;
        // This one tries to create useful types for a field of type 'object' (used for all unknown properties)
        yield return AnyObjectReader.Instance;
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
