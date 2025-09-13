using System.Collections.Generic;

using JetBrains.Annotations;

using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
internal abstract class ListenSubmissionPayload(string type) {

  public string Type { get; } = type;

}

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
internal abstract class ListenSubmissionPayload<T>(string type) : ListenSubmissionPayload(type) where T : ISubmittedListenData {

  public List<T> Listens { get; set; } = [ ];

}
