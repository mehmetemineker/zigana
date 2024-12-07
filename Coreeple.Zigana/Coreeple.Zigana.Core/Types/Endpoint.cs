using System.Text.Json.Nodes;

namespace Coreeple.Zigana.Core.Types;
public class Endpoint
{
    public Guid Id { get; set; }
    public string? Path { get; set; }
    public JsonObject? Defs { get; set; }
    public Request? Request { get; set; }
    public Dictionary<string, Action>? Actions { get; set; }
    public Dictionary<string, Response>? Response { get; set; }
}
