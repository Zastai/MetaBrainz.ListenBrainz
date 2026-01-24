using System;
using System.Collections.Generic;
using System.Text.Json;

using MetaBrainz.Common.Json;
using MetaBrainz.Common.Json.Converters;
using MetaBrainz.ListenBrainz.Interfaces;
using MetaBrainz.ListenBrainz.Objects;

namespace MetaBrainz.ListenBrainz.Json.Readers;

internal sealed class FoundFeedbackReader : ObjectReader<FoundFeedback> {

  public static readonly FoundFeedbackReader Instance = new();

  protected override FoundFeedback ReadObjectContents(ref Utf8JsonReader reader, JsonSerializerOptions options)  {
    int? count = null;
    int? offset = null;
    IReadOnlyList<IRecordingFeedback>? feedback = null;
    int? totalCount = null;
    Dictionary<string, object?>? rest = null;
    while (reader.TokenType == JsonTokenType.PropertyName) {
      var prop = reader.GetPropertyName();
      try {
        reader.Read();
        switch (prop) {
          case "count":
            count = reader.GetInt32();
            break;
          case "feedback":
            feedback = reader.ReadList(RecordingFeedbackReader.Instance, options);
            break;
          case "offset":
            offset = reader.GetInt32();
            break;
          case "total_count":
            totalCount = reader.GetInt32();
            break;
          default:
            rest ??= [ ];
            rest[prop] = reader.GetOptionalObject(options);
            break;
        }
      }
      catch (Exception e) {
        throw new JsonException($"Failed to deserialize the '{prop}' property.", e);
      }
      reader.Read();
    }
    return new FoundFeedback {
      Count = count ?? throw new MissingField("count"),
      Feedback = feedback ?? throw new MissingField("feedback"),
      Offset = offset ?? throw new MissingField("offset"),
      TotalCount = totalCount ?? throw new MissingField("playlist_count"),
      UnhandledProperties = rest,
    };
  }

}
