using System.Text.Json;

namespace MetaBrainz.ListenBrainz.Json.Readers;

internal sealed class MissingField(string field) : JsonException($"Expected '{field}' field not found or null.");
