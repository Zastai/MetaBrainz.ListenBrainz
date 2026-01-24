using System;

using JetBrains.Annotations;

using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

/// <inheritdoc cref="ISubmittedListen"/>
[PublicAPI]
public class SubmittedListen : SubmittedListenData, ISubmittedListen {

  /// <inheritdoc/>
  public DateTimeOffset Timestamp {
    get => DateTimeOffset.FromUnixTimeSeconds(this.UnixTimestamp);
    set => this.UnixTimestamp = value.ToUnixTimeSeconds();
  }

  /// <inheritdoc/>
  public long UnixTimestamp { get; set; }

}
