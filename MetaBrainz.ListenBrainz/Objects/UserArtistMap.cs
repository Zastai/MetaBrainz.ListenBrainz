using System.Collections.Generic;

using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

internal sealed class UserArtistMap : UserStatistics, IUserArtistMap {

  public IReadOnlyList<IArtistCountryInfo>? Countries { get; init; }

}
