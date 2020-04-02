using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

using JetBrains.Annotations;

using MetaBrainz.Common.Json;
using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects {

  [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
  internal sealed class FetchedListens : JsonBasedObject, IFetchedListens {

    [JsonPropertyName("count")]
    public int? Count { get; set; }

    [JsonPropertyName("listens")]
    public IReadOnlyList<IListen>? Listens { get; set; }

    [JsonPropertyName("playing_now")]
    public bool? PlayingNow { get; set; }

    [JsonConverter(typeof(UnixTime.JsonConverter))]
    [JsonPropertyName("latest_listen_ts")]
    public DateTimeOffset? Timestamp { get; set; }

    public long? UnixTimestamp => UnixTime.Convert(this.Timestamp);

    [JsonPropertyName("user_id")]
    public string? User { get; set; }

    [JsonPropertyName("user_list")]
    public string? UserList { get; set; }

  }

}
