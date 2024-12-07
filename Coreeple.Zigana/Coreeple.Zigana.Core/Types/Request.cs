using System.Text.Json.Nodes;

namespace Coreeple.Zigana.Core.Types;
public class Request
{
    public JsonObject? RouteParameters { get; set; }
    public JsonObject? QueryParameters { get; set; }
    public JsonObject? Headers { get; set; }
    public JsonNode? Body { get; set; }
}
