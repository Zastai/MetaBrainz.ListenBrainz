using System.Collections.Generic;
using System.Text.Json;

namespace MetaBrainz.ListenBrainz.Json.Readers;

internal static class Helpers {

  public static IReadOnlyList<TItem> VerifyPayloadContents<TItem>(this IReadOnlyList<TItem>? items, int? count) {
    if (count is null) {
      throw new JsonException("Expected payload item count not found or null.");
    }
    var n = items?.Count ?? 0;
    if (n != count.Value) {
      throw new JsonException($"The size of the list of items ({n}) does not match the reported item count ({count}).");
    }
    // treat missing/null the same as []
    return items ?? [ ];
  }

}
