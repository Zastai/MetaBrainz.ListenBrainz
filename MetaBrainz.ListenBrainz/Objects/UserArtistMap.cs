using System;
using System.Collections.Generic;

using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

internal sealed class UserArtistMap : UserStatistics, IUserArtistMap {

  public UserArtistMap(DateTimeOffset lastUpdated, StatisticsRange range, string user)
  : base(lastUpdated, range, user)
  { }

  public IReadOnlyList<IArtistCountryInfo>? Countries { get; init; }

}
