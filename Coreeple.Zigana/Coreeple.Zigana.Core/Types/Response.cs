using System.Text.Json.Nodes;

namespace Coreeple.Zigana.Core.Types;
public class Response
{
    public JsonNode? When { get; set; }
    public int HttpStatusCode { get; set; }
    public JsonObject? Headers { get; set; }
    public JsonNode? Content { get; set; }
}
