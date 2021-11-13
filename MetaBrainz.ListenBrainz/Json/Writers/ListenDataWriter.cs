using System.Text.Json;

using MetaBrainz.Common.Json.Converters;
using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Json.Writers;

internal sealed class ListenDataWriter : ObjectWriter<ISubmittedListenData> {

  public static readonly ListenDataWriter Instance = new ListenDataWriter();

  protected override void WriteObjectContents(Utf8JsonWriter writer, ISubmittedListenData value, JsonSerializerOptions options) {
    writer.WritePropertyName("track_metadata");
    TrackInfoWriter.Instance.Write(writer, value.Track, options);
  }

}
