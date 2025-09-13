using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

internal sealed class GenreActivityDetails : HourlyActivity, IGenreActivityDetails {

  public required string Genre { get; init; }

}
