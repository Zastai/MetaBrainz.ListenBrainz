using System;

using JetBrains.Annotations;

using MetaBrainz.Common.Json;
using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
internal sealed class Listen : JsonBasedObject, IListen {

  public Listen(long inserted, long listened, Guid msid, ITrackInfo track, string user) {
    this.InsertedAt = DateTimeOffset.FromUnixTimeSeconds(inserted);
    this.ListenedAt = DateTimeOffset.FromUnixTimeSeconds(listened);
    this.MessyRecordingId = msid;
    this.Track = track;
    this.User = user;
  }

  public DateTimeOffset InsertedAt { get; }

  public DateTimeOffset ListenedAt { get; }

  public Guid MessyRecordingId { get; }

  public ITrackInfo Track { get; }

  public string User { get; }

}
