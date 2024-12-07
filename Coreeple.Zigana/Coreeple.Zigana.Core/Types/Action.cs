using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Coreeple.Zigana.Core.Types;

[JsonDerivedType(typeof(HttpRequestAction))]
public class Action
{
    public required string Type { get; set; }
    public JsonNode? When { get; set; }
    public JsonNode? Input { get; set; }
    public JsonNode? Output { get; set; }
}

