using System.Collections.Generic;
using MetaBrainz.ListenBrainz.Interfaces;
using Newtonsoft.Json;

namespace MetaBrainz.ListenBrainz.Objects {

  [JsonObject]
  internal sealed class FetchedListens : IFetchedListens {

    [JsonProperty("count", Required = Required.Always)]
    public int Count { get; private set; }

    [JsonIgnore]
    public IReadOnlyList<IListen> Listens => _listens;

    [JsonProperty("listens", Required = Required.Always)]
    private Listen[] _listens = null;

    [JsonProperty("user_id", Required = Required.Always)]
    public string User { get; private set; }

  }

}
