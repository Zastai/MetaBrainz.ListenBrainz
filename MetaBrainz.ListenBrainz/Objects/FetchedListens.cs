using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

using JetBrains.Annotations;

using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects {

  internal sealed class FetchedListens : JsonBasedObject, IFetchedListens {

    [JsonPropertyName("count")]
    [UsedImplicitly]
    public int Count { get; set; }

    public IReadOnlyList<IListen> Listens => this.TheListens;

    [JsonPropertyName("listens")]
    [UsedImplicitly]
    public IReadOnlyList<Listen> TheListens { get; set; }

    [JsonIgnore]
    public DateTime Timestamp { get; private set; }

    [JsonPropertyName("latest_listen_ts")]
    [UsedImplicitly]
    public long UnixTimestamp {
      get => this._unixTimestamp;
      set => this.Timestamp = UnixTime.Convert(this._unixTimestamp = value);
    }

    private long _unixTimestamp = 0;

    [JsonPropertyName("user_id")]
    [UsedImplicitly]
    public string User { get; set; }

  }

}
