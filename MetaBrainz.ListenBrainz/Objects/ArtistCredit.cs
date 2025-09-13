using System;

using MetaBrainz.Common.Json;
using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

internal sealed class ArtistCredit : JsonBasedObject, IArtistCredit {

  public required string CreditedName { get; init; }

  public required Guid Id { get; init; }

  public required string JoinPhrase { get; init; }

}
