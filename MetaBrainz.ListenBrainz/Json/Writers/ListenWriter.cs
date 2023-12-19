using System.Text.Json;

using MetaBrainz.Common.Json.Converters;
using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Json.Writers;

internal sealed class ListenWriter : ObjectWriter<ISubmittedListen> {

  public static readonly ListenWriter Instance = new();

  protected override void WriteObjectContents(Utf8JsonWriter writer, ISubmittedListen value, JsonSerializerOptions options) {
    writer.WriteNumber("listened_at", value.Timestamp.ToUnixTimeSeconds());
    writer.WritePropertyName("track_metadata");
    TrackInfoWriter.Instance.Write(writer, value.Track, options);
  }

}
