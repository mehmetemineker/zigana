using System.Text.Json.Nodes;

namespace Zigana.Core.Types;
public class Response
{
    public JsonNode? When { get; set; }
    public JsonObject? Headers { get; set; }
    public JsonNode? Content { get; set; }
}
