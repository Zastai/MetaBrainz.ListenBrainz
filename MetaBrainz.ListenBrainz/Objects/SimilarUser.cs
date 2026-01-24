using MetaBrainz.Common.Json;
using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

internal sealed class SimilarUser : JsonBasedObject, ISimilarUser {

  public override int GetHashCode() => this.Name.GetHashCode();

  public required string Name { get; init; }

  public required decimal Similarity { get; init; }

}
