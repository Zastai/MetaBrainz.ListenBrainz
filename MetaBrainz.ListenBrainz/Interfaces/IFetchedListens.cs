using System;
using System.Collections.Generic;

using JetBrains.Annotations;

using MetaBrainz.Common.Json;

namespace MetaBrainz.ListenBrainz.Interfaces {

  /// <summary>Information about a set of fetched listens.</summary>
  /// <remarks>
  /// Which fields are set, aside from <see cref="Count"/> and <see cref="Listens"/>, which should always be present, depends on
  /// the kind of listen request.<br/>
  /// In the case of <see cref="ListenBrainz.GetPlayingNow(string)"/>, <see cref="PlayingNow"/> will be set to
  /// <see langword="true"/> and <see cref="User"/> will be set to the specified user name.<br/>
  /// When <see cref="ListenBrainz.GetRecentListens(string[])"/> is used, only <see cref="UserList"/> will be set, to the
  /// specified list of user, separated by commas.<br/>
  /// In the most common case, using <see cref="ListenBrainz.GetListens(string,int?)"/>, <see cref="User"/> will be set to the
  /// specified user name, and <see cref="Timestamp"/>/<see cref="UnixTimestamp"/> will be set to the timestamp of the most recent
  /// listen included in <see cref="Listens"/> (or <see cref="UnixTime.Epoch"/> if no listens were returned).
  /// </remarks>
  [PublicAPI]
  public interface IFetchedListens : IJsonBasedObject {

    /// <summary>The number of listens fetched.</summary>
    int? Count { get; }

    /// <summary>The listens that were fetched.</summary>
    IReadOnlyList<IListen>? Listens { get; }

    /// <summary>Indicates whether this set of listens represents a currently playing track.</summary>
    bool? PlayingNow { get; }

    /// <summary>The timestamp of the newest listen included in <see cref="Listens"/>.</summary>
    DateTimeOffset? Timestamp { get; }

    /// <summary>
    /// The timestamp of the newest listen included in <see cref="Listens"/>, expressed as the number of seconds since
    /// <see cref="UnixTime.Epoch">the Unix time epoch</see>.
    /// </summary>
    long? UnixTimestamp { get; }

    /// <summary>The MusicBrainz ID of the user for which the listens were fetched.</summary>
    string? User { get; }

    /// <summary>A comma-separated list of the MusicBrainz IDs of the users for which the listens were fetched.</summary>
    string? UserList { get; }

  }

}
