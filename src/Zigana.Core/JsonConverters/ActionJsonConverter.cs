using Zigana.Core.Types;
using System.Text.Json;
using System.Text.Json.Serialization;
using Action = Zigana.Core.Types.Action;

namespace Zigana.Core.JsonConverters;
public class ActionJsonConverter : JsonConverter<Action>
{
    public override Action? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var jsonDoc = JsonDocument.ParseValue(ref reader);
        var root = jsonDoc.RootElement;
        var type = root.GetProperty("type").GetString();

        return type switch
        {
            "http-request" => root.Deserialize<HttpRequestAction>(options),
            "html-parser" => root.Deserialize<HtmlParserAction>(options),
            "parallel" => root.Deserialize<ParallelAction>(options),
            _ => throw new NotSupportedException($"Unsupported action type: {type}")
        };
    }

    public override void Write(Utf8JsonWriter writer, Types.Action value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, (object)value, options);
    }
}