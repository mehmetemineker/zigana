using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Coreeple.Zigana.Core.Types;

//[JsonConverter(typeof(ActionJsonConverter))]
[JsonDerivedType(typeof(HttpRequestAction))]
[JsonDerivedType(typeof(HtmlParserAction))]
[JsonDerivedType(typeof(ParallelAction))]
public class Action
{
    public required string Type { get; set; }
    public JsonNode? When { get; set; }
}

