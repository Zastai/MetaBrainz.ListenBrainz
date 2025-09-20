using System;

namespace MetaBrainz.ListenBrainz.Interfaces.JSPF;

/// <summary>A resource link.</summary>
public interface ILink {

  /// <summary>The URI identifying the resource type.</summary>
  Uri Id { get; }

  /// <summary>The URI identifying the resource.</summary>
  Uri Value { get; }

}
