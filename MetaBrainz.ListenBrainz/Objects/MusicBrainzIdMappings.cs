using System;
using System.Collections.Generic;

using JetBrains.Annotations;

using MetaBrainz.Common.Json;
using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
internal sealed class MusicBrainzIdMappings : JsonBasedObject, IMusicBrainzIdMappings {
  
  public IReadOnlyList<Guid>? ArtistIds { get; set; }

  public Guid? RecordingId { get; set; }

  public Guid? ReleaseId { get; set; }

}
