using System;
using System.Collections.Generic;
using System.Text.Json;

using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Json.Readers {

  internal abstract class ListenPayloadReader<T> : PayloadReader<T> {

    protected IReadOnlyList<IListen> VerifyListens(int? count, IReadOnlyList<IListen>? listens) {
      if (!count.HasValue)
        throw new JsonException("Expected listen count not found or null.");
      var n = listens?.Count ?? 0;
      if (n != count.Value)
        throw new JsonException($"The size of the list of listens ({n}) does not match the reported listen count ({count}).");
      return listens ?? Array.Empty<IListen>(); // treat missing/null the same as []
    }

  }

}
