using System;

using MetaBrainz.ListenBrainz.Interfaces.JSPF;

namespace MetaBrainz.ListenBrainz.Objects.JSPF;

internal sealed class Meta : IMeta {

  public required Uri Id { get; init; }

  public required string Value { get; init; }

}
