using System.Text.Json.Nodes;

namespace Coreeple.Zigana.Core.Types;
public class Response
{
    public JsonNode? When { get; set; }
    public Dictionary<string, object>? Headers { get; set; }
    public JsonNode? Content { get; set; }
}
