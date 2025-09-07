using MetaBrainz.Common.Json;
using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

internal sealed class TopListener : JsonBasedObject, ITopListener {

  public required int ListenCount { get; init; }

  public required string UserName { get; init; }

}
