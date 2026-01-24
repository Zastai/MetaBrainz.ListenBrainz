using System.Text.Json;

using MetaBrainz.Common.Json.Converters;
using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Json.Writers;

internal sealed class RecordingFeedbackWriter : ObjectWriter<ISubmittedRecordingFeedback> {

  public static readonly RecordingFeedbackWriter Instance = new();

  protected override void WriteObjectContents(Utf8JsonWriter writer, ISubmittedRecordingFeedback value,
                                              JsonSerializerOptions options) {
    if (value.Id is not null) {
      writer.WriteString("recording_mbid", value.Id.Value);
    }
    if (value.MessyId is not null) {
      writer.WriteString("recording_msid", value.MessyId.Value);
    }
    writer.WriteNumber("score", value.Score);
  }

}
