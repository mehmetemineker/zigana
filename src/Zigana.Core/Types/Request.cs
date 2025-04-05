using System.Text.Json.Nodes;

namespace Zigana.Core.Types;
public class Request
{
    public JsonObject Route { get; set; } = [];
    public JsonObject Query { get; set; } = [];
    public JsonObject Headers { get; set; } = [];
    public JsonNode Body { get; set; } = JsonNode.Parse("{}")!;
}
