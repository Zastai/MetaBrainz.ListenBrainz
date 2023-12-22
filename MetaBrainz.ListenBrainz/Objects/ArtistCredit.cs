using System;
using System.Collections.Generic;

using MetaBrainz.Common.Json;
using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

internal sealed class ArtistCredit : JsonBasedObject, IArtistCredit {

  public ArtistCredit(string creditedName, Guid id, string joinPhrase) {
    this.CreditedName = creditedName;
    this.Id = id;
    this.JoinPhrase = joinPhrase;
  }

  public string CreditedName { get; }

  public Guid Id { get; }

  public string JoinPhrase { get; }

}
