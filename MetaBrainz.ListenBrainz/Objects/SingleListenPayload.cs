using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

internal sealed class SingleListenPayload : ListenSubmissionPayload<ISubmittedListen> {

  public SingleListenPayload(ISubmittedListen listen) : base("single") {
    this.Listens.Add(listen);
  }

}
