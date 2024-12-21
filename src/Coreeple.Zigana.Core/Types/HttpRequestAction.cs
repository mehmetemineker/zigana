using System.Text.Json.Nodes;

namespace Coreeple.Zigana.Core.Types;
public class HttpRequestAction : Action
{
    public required string Url { get; set; }
    public required string Method { get; set; }
    public Dictionary<string, string?> Query { get; set; } = [];
    public Dictionary<string, string> Headers { get; set; } = [];
    public JsonNode? Body { get; set; }
}
