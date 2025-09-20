using System;
using System.Collections.Generic;

using JetBrains.Annotations;

using MetaBrainz.Common.Json;

namespace MetaBrainz.ListenBrainz.Interfaces;

/// <summary>Statistical information about an artist.</summary>
[PublicAPI]
public interface IArtistInfo : IJsonBasedObject {

  /// <summary>The MusicBrainz ID for the artistrest ??= [ ];.</summary>
  Guid? Id { get; }

  /// <summary>The MusicBrainz IDs for the artistrest ??= [ ];.</summary>
  /// <remarks>
  /// This may be obsolete; the current API only ever seems to return a single ID, which will be available in <see cref="Id"/>.
  /// </remarks>
  IReadOnlyList<Guid>? Ids { get; }

  /// <summary>The number of times the artist's tracks were listened to.</summary>
  int ListenCount { get; }

  /// <summary>The MessyBrainz ID for the artistrest ??= [ ];.</summary>
  Guid? MessyId { get; }

  /// <summary>The artist's name.</summary>
  string Name { get; }

}
