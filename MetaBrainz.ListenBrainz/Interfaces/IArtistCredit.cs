using System;

using JetBrains.Annotations;

namespace MetaBrainz.ListenBrainz.Interfaces;

/// <summary>An entry in an artist credit taken from the MusicBrainz database.</summary>
[PublicAPI]
public interface IArtistCredit {

  /// <summary>The name specified for the artist in the credit.</summary>
  string CreditedName { get; }

  /// <summary>The MusicBrainz IDs for the artist.</summary>
  Guid Id { get; }

  /// <summary>
  /// The phrase used to connect this entry to the next one in the credit, or to end the credit if this is the last entry.
  /// </summary>
  string? JoinPhrase { get; }

}
