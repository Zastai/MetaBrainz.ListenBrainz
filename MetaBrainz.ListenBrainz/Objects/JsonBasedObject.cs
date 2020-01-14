using System.Collections.Generic;
using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace MetaBrainz.ListenBrainz.Objects {

  internal abstract class JsonBasedObject {

    [UsedImplicitly]
    public IReadOnlyDictionary<string, object> UnhandledProperties => this._unhandledProperties ??= JsonUtils.Unwrap(this.TheUnhandledProperties);

    private Dictionary<string, object> _unhandledProperties;

    [JsonExtensionData]
    [UsedImplicitly]
    public Dictionary<string, object> TheUnhandledProperties { get; set; }

  }

}
