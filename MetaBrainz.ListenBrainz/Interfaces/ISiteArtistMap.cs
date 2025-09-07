using System.Collections.Generic;

using JetBrains.Annotations;

namespace MetaBrainz.ListenBrainz.Interfaces;

/// <summary>Information about the number of times artists are listened to, grouped by their country.</summary>
[PublicAPI]
public interface ISiteArtistMap : IArtistMap, IStatistics;
