using MetaBrainz.Common.Json;
using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

internal sealed class YearInMusic : JsonBasedObject, IYearInMusic {

  public required IYearInMusicData Data { get; init; }

  public required string User { get; init; }

}
