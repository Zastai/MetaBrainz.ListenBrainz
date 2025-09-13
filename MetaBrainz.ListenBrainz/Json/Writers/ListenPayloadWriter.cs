using System.Text.Json;

using MetaBrainz.Common.Json;
using MetaBrainz.Common.Json.Converters;
using MetaBrainz.ListenBrainz.Interfaces;
using MetaBrainz.ListenBrainz.Objects;

namespace MetaBrainz.ListenBrainz.Json.Writers;

internal sealed class ListenPayloadWriter : ObjectWriter<ListenSubmissionPayload<ISubmittedListen>> {

  public static readonly ListenPayloadWriter Instance = new();

  protected override void WriteObjectContents(Utf8JsonWriter writer, ListenSubmissionPayload<ISubmittedListen> value,
                                              JsonSerializerOptions options) {
    writer.WriteString("listen_type", value.Type);
    switch (value.Type) {
      case "import":
      case "single":
        writer.WritePropertyName("payload");
        writer.WriteList(value.Listens, ListenWriter.Instance, options);
        break;
      default:
        throw new JsonException($"Invalid listen submission payload type: '{value.Type}'.");
    }
  }

}
