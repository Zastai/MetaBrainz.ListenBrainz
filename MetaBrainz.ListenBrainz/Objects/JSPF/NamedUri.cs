using System;

using MetaBrainz.ListenBrainz.Interfaces.JSPF;

namespace MetaBrainz.ListenBrainz.Objects.JSPF;

internal sealed class NamedUri : INamedUri {

  public required string Name { get; init; }

  public required Uri Uri { get; init; }

}
