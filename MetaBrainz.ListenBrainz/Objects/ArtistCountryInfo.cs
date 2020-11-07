using MetaBrainz.Common.Json;
using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects {

  internal sealed  class ArtistCountryInfo : JsonBasedObject, IArtistCountryInfo {

    public ArtistCountryInfo(int artistCount, string country, int listenCount) {
      this.ArtistCount = artistCount;
      this.Country = country;
      this.ListenCount = listenCount;
    }

    public int ArtistCount { get; }

    public string Country { get; }

    public int ListenCount { get; }

  }

}
