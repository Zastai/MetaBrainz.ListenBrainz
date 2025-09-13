using System;

using MetaBrainz.Common.Json;
using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

internal sealed class ListenTimeRange : JsonBasedObject, IListenTimeRange {

  public required string Description { get; init; }

  public required int ListenCount { get; init; }

  public DateTimeOffset? RangeEnd { get; init; }

  public DateTimeOffset? RangeStart { get; init; }

}
