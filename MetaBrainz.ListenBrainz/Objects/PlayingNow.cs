using JetBrains.Annotations;

using MetaBrainz.Common.Json;
using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
internal sealed class PlayingNow : JsonBasedObject, IPlayingNow {

  public PlayingNow(string user) {
    this.User = user;
  }

  public IPlayingTrack? Track { get; init; }

  public string User { get; }

}
