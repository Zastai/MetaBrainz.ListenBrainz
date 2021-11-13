using JetBrains.Annotations;

using MetaBrainz.Common.Json;
using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
internal sealed class PlayingNow : JsonBasedObject, IPlayingNow {

  public PlayingNow(IPlayingTrack? track, string user) {
    this.Track = track;
    this.User = user;
  }

  public IPlayingTrack? Track { get; }

  public string User { get; }

}
