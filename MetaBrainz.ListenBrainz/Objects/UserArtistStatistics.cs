using System.Collections.Generic;

using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

internal sealed class UserArtistStatistics : UserStatistics, IUserArtistStatistics {

  public IReadOnlyList<IArtistInfo>? Artists { get; init; }

  public required int Count { get; init; }

  public required int Offset { get; init; }

  public required int TotalCount { get; init; }

}
