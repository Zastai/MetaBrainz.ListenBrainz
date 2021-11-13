using System;
using System.Collections.Generic;

using MetaBrainz.Common.Json;
using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

internal sealed class ArtistTimeRange : JsonBasedObject, IArtistTimeRange {

  public ArtistTimeRange(string description) {
    this.Description = description;
  }

  public string Description { get; }

  public IReadOnlyList<IArtistInfo>? Artists { get; set; }

  public DateTimeOffset? RangeEnd { get; set; }

  public DateTimeOffset? RangeStart { get; set; }

}
