using System.Text.Json.Nodes;

namespace Coreeple.Zigana.Core.Types;

public class Action
{
    public required string Type { get; set; }
    public string? Name { get; set; }
    public JsonNode? When { get; set; }
    public JsonNode? Input { get; set; }
    public JsonNode? Output { get; set; }
}

