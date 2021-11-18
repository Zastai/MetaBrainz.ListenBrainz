using System.Text.Json;

using MetaBrainz.Common.Json.Converters;
using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Json.Writers;

internal sealed class TrackInfoWriter : ObjectWriter<ISubmittedTrackInfo> {

  public static readonly TrackInfoWriter Instance = new();

  protected override void WriteObjectContents(Utf8JsonWriter writer, ISubmittedTrackInfo value, JsonSerializerOptions options) {
    writer.WriteString("artist_name", value.Artist);
    writer.WriteString("track_name", value.Name);
    {
      var release = value.Release;
      if (release != null) {
        writer.WriteString("release_name", release);
      }
    }
    {
      var extra = value.AdditionalInfo;
      if (extra != null) {
        writer.WritePropertyName("additional_info");
        JsonSerializer.Serialize(writer, extra, extra.GetType(), options);
      }
    }
  }

}
