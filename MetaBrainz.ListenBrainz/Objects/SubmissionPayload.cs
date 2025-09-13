using System.Collections.Generic;

using JetBrains.Annotations;

using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
internal abstract class SubmissionPayload(string type) {

  public string Type { get; } = type;

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
internal sealed class SubmissionPayload<T>(string type) : SubmissionPayload(type) where T : ISubmittedListenData {

  public List<T> Listens { get; } = [ ];

}
