using MetaBrainz.Common.Json;

namespace MetaBrainz.ListenBrainz.Interfaces;

/// <summary>Information about a user's "Year in Music".</summary>
public interface IYearInMusic : IJsonBasedObject {

  /// <summary>The "Year in Music" data.</summary>
  IYearInMusicData Data { get; }

  /// <summary>The user for whom the information was computed.</summary>
  string User { get; }

}
