using System;

using MetaBrainz.Common.Json;
using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

internal sealed class ListenTimeRange : JsonBasedObject, IListenTimeRange {

  public ListenTimeRange(string description, int listenCount) {
    this.Description = description;
    this.ListenCount = listenCount;
  }

  public string Description { get; }

  public int ListenCount { get; }

  public DateTimeOffset? RangeEnd { get; init; }

  public DateTimeOffset? RangeStart { get; init; }

}
