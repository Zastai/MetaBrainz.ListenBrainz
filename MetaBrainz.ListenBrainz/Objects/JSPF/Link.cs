using System;

using MetaBrainz.ListenBrainz.Interfaces.JSPF;

namespace MetaBrainz.ListenBrainz.Objects.JSPF;

internal sealed class Link : ILink {

  public required Uri Id { get; init; }

  public required Uri Value { get; init; }

}
