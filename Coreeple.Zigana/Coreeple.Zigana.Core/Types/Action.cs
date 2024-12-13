using Coreeple.Zigana.Core.Types.Actions;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Coreeple.Zigana.Core.Types;

[JsonDerivedType(typeof(HttpRequestAction))]
[JsonDerivedType(typeof(HtmlParserAction))]
public class Action
{
    public required string Type { get; set; }
    public JsonNode? When { get; set; }
}

