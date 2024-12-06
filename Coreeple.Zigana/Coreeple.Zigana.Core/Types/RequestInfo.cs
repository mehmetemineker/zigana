using System.Text.Json.Nodes;

namespace Coreeple.Zigana.Core.Types;
public class RequestInfo
{
    public Guid ApiId { get; set; }
    public Guid EndpointId { get; set; }
    public JsonObject? Definitions { get; set; }
    public JsonObject? RouteParameters { get; set; }
    public JsonObject? QueryParameters { get; set; }
    public JsonObject? Headers { get; set; }
    public JsonNode? Body { get; set; }
    public IEnumerable<Action>? Actions { get; set; }
    public Dictionary<string, Response>? Response { get; set; }
}
