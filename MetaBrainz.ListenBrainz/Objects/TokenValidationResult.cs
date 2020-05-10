using MetaBrainz.Common.Json;

namespace MetaBrainz.ListenBrainz.Objects {

  internal sealed class TokenValidationResult : JsonBasedObject {

    public int? Code { get; set; }

    public string? Message { get; set; }

    public string? User { get; set; }

    public bool? Valid { get; set; }

  }

}
