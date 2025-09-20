using System;

namespace MetaBrainz.ListenBrainz.Interfaces.JSPF;

/// <summary>A metadata item.</summary>
public interface IMeta {

  /// <summary>The URI identifying the metadata.</summary>
  Uri Id { get; }

  /// <summary>The value for the metadata.</summary>
  string Value { get; }

}
