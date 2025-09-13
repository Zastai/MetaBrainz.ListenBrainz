using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects;

internal sealed class ImportPayload() : ListenSubmissionPayload<ISubmittedListen>("import");
