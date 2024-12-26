using Coreeple.Zigana.EndpointProcessor.Abstractions;
using System.Text.Json.Nodes;

namespace Coreeple.Zigana.EndpointProcessor;
public class EndpointContext : IEndpointContext
{
    private readonly JsonObject _context;
    private Guid Id = Guid.NewGuid();
    private string? RequestId;

    public EndpointContext()
    {
        _context = new JsonObject
        {
            ["id"] = Guid.NewGuid(),
            ["requestId"] = Guid.NewGuid(),
            ["defs"] = new JsonObject(),
            ["request"] = new JsonObject()
            {
                ["query"] = new JsonObject(),
                ["headers"] = new JsonObject(),
                ["body"] = new JsonObject(),
                ["route"] = new JsonObject()
            },
            ["actions"] = new JsonObject(),
            ["response"] = new JsonObject(),
        };
    }

    public void SetId(Guid value) => Id = value;
    public void SetRequestId(string? value) => RequestId = value;
    public void SetDefs(JsonObject value) => _context["defs"] = value;
    public void SetRequestQuery(JsonObject value) => _context["request"]!["query"] = value;
    public void SetRequestHeaders(JsonObject value) => _context["request"]!["headers"] = value;
    public void SetRequestBody(JsonNode value) => _context["request"]!["body"] = value;
    public void SetRequestRoute(JsonObject value) => _context["request"]!["route"] = value;
    public void AddAction(string key, JsonObject value) => _context["actions"]![key] = value;
    public void SetResponse(JsonObject value) => _context["response"] = value;
    public JsonObject Get() => _context;
    public Guid GetId() => Id;
    public string? GetRequestId() => RequestId;
    public JsonObject GetResponse() => _context["response"]!.AsObject();
}
