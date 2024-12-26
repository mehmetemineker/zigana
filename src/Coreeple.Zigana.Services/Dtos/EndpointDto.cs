using Coreeple.Zigana.Core.Types;
using System.Text.Json.Nodes;
using Action = Coreeple.Zigana.Core.Types.Action;

namespace Coreeple.Zigana.Services.Dtos;
public class EndpointDto
{
    public Guid Id { get; set; }
    public string? Path { get; set; }
    public JsonObject Defs { get; set; } = [];
    public Request Request { get; set; } = new();
    public Dictionary<string, Action>? Actions { get; set; }
    public Dictionary<string, Response>? Responses { get; set; }
}
