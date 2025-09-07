using System;

using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

internal abstract class UserStatistics : Statistics, IUserStatistics {

  public required string User { get; init; }

}
