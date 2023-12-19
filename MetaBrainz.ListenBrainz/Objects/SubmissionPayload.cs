using System.Collections.Generic;

using JetBrains.Annotations;

using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
internal abstract class SubmissionPayload {

  protected SubmissionPayload(string type) {
    this.Type = type;
  }

  public string Type { get; }

  public static SubmissionPayload<ISubmittedListen> CreateImport() => new("import");

  public static SubmissionPayload<ISubmittedListenData> CreatePlayingNow(ISubmittedListenData listen) {
    var payload = new SubmissionPayload<ISubmittedListenData>("playing_now");
    payload.Listens.Add(listen);
    return payload;
  }

  public static SubmissionPayload<ISubmittedListen> CreateSingle(ISubmittedListen listen) {
    var payload = new SubmissionPayload<ISubmittedListen>("single");
    payload.Listens.Add(listen);
    return payload;
  }

}

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
internal sealed class SubmissionPayload<T> : SubmissionPayload where T : ISubmittedListenData {

  public SubmissionPayload(string type) : base(type) {
  }

  public List<T> Listens { get; } = new();

}
