using System;
using System.Text.Json;
using System.Text.Json.Serialization;

using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects {

  internal sealed class SubmissionSerializer : JsonConverterFactory {

    private abstract class JsonSerializer<T> : JsonConverter<T> {

      public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => throw new InvalidOperationException("This converter is for serialization only.");

    }

    private sealed class Listen : JsonSerializer<ISubmittedListen> {

      public override void Write(Utf8JsonWriter writer, ISubmittedListen value, JsonSerializerOptions options) {
        writer.WriteStartObject();
        writer.WriteNumber("listened_at", UnixTime.Convert(value.Timestamp));
        writer.WritePropertyName("track_metadata");
        JsonSerializer.Serialize(writer, value.Track, options);
        writer.WriteEndObject();
      }

    }

    private sealed class ListenData : JsonSerializer<ISubmittedListenData> {

      public override void Write(Utf8JsonWriter writer, ISubmittedListenData value, JsonSerializerOptions options) {
        writer.WriteStartObject();
        writer.WritePropertyName("track_metadata");
        JsonSerializer.Serialize(writer, value.Track, options);
        writer.WriteEndObject();
      }

    }

    private sealed class TrackInfo : JsonSerializer<ISubmittedTrackInfo> {

      public override void Write(Utf8JsonWriter writer, ISubmittedTrackInfo value, JsonSerializerOptions options) {
        writer.WriteStartObject();
        writer.WriteString("artist_name", value.Artist);
        writer.WriteString("track_name", value.Name);
        {
          var release = value.Release;
          if (release != null)
            writer.WriteString("release_name", release);
        }
        {
          var extra = value.AdditionalInfo;
          if (extra != null) {
            writer.WritePropertyName("additional_info");
            JsonSerializer.Serialize(writer, extra, extra.GetType(), options);
          }
        }
        writer.WriteEndObject();
      }

    }

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options) {
      if (typeof(ISubmittedListen).IsAssignableFrom(typeToConvert))
        return new Listen();
      if (typeof(ISubmittedListenData).IsAssignableFrom(typeToConvert))
        return new ListenData();
      if (typeof(ISubmittedTrackInfo).IsAssignableFrom(typeToConvert))
        return new TrackInfo();
      throw new NotSupportedException($"This converter cannot handle objects of type {typeToConvert}.");
    }

    public override bool CanConvert(Type typeToConvert) {
      if (typeof(ISubmittedListen).IsAssignableFrom(typeToConvert))
        return true;
      if (typeof(ISubmittedListenData).IsAssignableFrom(typeToConvert))
        return true;
      if (typeof(ISubmittedTrackInfo).IsAssignableFrom(typeToConvert))
        return true;
      return false;
    }

  }

}
