using System;

namespace MetaBrainz.ListenBrainz.Interfaces.JSPF;

/// <summary>A named URI.</summary>
public interface INamedUri {

  /// <summary>The name associated with the URI.</summary>
  string Name { get; }

  /// <summary>The URI.</summary>
  Uri Uri { get; }

}
