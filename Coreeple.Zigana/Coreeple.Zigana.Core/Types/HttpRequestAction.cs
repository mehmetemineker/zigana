using System.Text.Json.Nodes;

namespace Coreeple.Zigana.Core.Types;
public class HttpRequestAction : Action
{
    public required string Url { get; set; }
    public required string Method { get; set; }
    public JsonObject? Query { get; set; }
    public JsonObject? Headers { get; set; }
    public JsonNode? Body { get; set; }
}
