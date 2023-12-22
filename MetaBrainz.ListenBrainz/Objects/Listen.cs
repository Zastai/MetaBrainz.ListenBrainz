using System;

using JetBrains.Annotations;

using MetaBrainz.Common.Json;
using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
internal sealed class Listen : JsonBasedObject, IListen {

  public Listen(long inserted, Guid msid, long timestamp, ITrackInfo track, string user) {
    this.InsertedAt = DateTimeOffset.FromUnixTimeSeconds(inserted);
    this.MessyRecordingId = msid;
    this.Timestamp = DateTimeOffset.FromUnixTimeSeconds(timestamp);
    this.Track = track;
    this.UnixTimestamp = timestamp;
    this.User = user;
  }

  public DateTimeOffset InsertedAt { get; }

  public Guid MessyRecordingId { get; }

  public DateTimeOffset Timestamp { get; }

  public ITrackInfo Track { get; }

  public long UnixTimestamp { get; }

  public string User { get; }

}
