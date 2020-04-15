using System;

using JetBrains.Annotations;

using MetaBrainz.Common.Json;
using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects {

  [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
  internal sealed class Listen : JsonBasedObject, IListen {

    public Listen(Guid msid, long timestamp, ITrackInfo track, string user) {
      this.MessyRecordingId = msid;
      this.Timestamp = UnixTime.Convert(timestamp);
      this.Track = track;
      this.UnixTimestamp = timestamp;
      this.User = user;
    }

    public Guid MessyRecordingId { get; }

    public DateTimeOffset Timestamp { get; }

    public ITrackInfo Track { get; }

    public long UnixTimestamp { get; }

    public string User { get; }

  }

}
