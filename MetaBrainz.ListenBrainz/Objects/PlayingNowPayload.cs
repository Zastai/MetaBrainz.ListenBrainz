using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

internal sealed class PlayingNowPayload : ListenSubmissionPayload<ISubmittedListenData> {

  public PlayingNowPayload(ISubmittedListenData listen) : base("playing_now") {
    this.Listens.Add(listen);
  }

}
