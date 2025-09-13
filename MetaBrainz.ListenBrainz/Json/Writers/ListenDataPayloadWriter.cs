using System.Text.Json;

using MetaBrainz.Common.Json;
using MetaBrainz.Common.Json.Converters;
using MetaBrainz.ListenBrainz.Interfaces;
using MetaBrainz.ListenBrainz.Objects;

namespace MetaBrainz.ListenBrainz.Json.Writers;

internal sealed class ListenDataPayloadWriter : ObjectWriter<ListenSubmissionPayload<ISubmittedListenData>> {

  public static readonly ListenDataPayloadWriter Instance = new();

  protected override void WriteObjectContents(Utf8JsonWriter writer, ListenSubmissionPayload<ISubmittedListenData> value,
                                              JsonSerializerOptions options) {
    writer.WriteString("listen_type", value.Type);
    switch (value.Type) {
      case "playing_now":
        writer.WritePropertyName("payload");
        writer.WriteList(value.Listens, ListenDataWriter.Instance, options);
        break;
      default:
        throw new JsonException($"Invalid submission payload type: '{value.Type}'.");
    }
  }

}
