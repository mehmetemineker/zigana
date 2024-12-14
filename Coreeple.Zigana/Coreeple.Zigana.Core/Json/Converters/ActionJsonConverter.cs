using Coreeple.Zigana.Core.Types.Actions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Coreeple.Zigana.Core.Json.Converters;
public class ActionJsonConverter : JsonConverter<Types.Action>
{
    public override Types.Action? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
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