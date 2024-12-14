using Coreeple.Zigana.Core.Abstractions;
using System.Text.Json.Nodes;

namespace Coreeple.Zigana.Core.ActionExecutors;
public class EndpointContext : IEndpointContext
{
    private readonly JsonObject _context;

    public EndpointContext()
    {
        _context = new JsonObject
        {
            ["defs"] = new JsonObject(),
            ["request"] = new JsonObject()
            {
                ["query"] = new JsonObject(),
                ["headers"] = new JsonObject(),
                ["body"] = new JsonObject(),
                ["route"] = new JsonObject()
            },
            ["actions"] = new JsonObject(),
            ["response"] = new JsonObject()
        };
    }

    public JsonObject Get() => _context;

    public void SetDefs(JsonObject value) => _context["defs"] = value;

    public void SetRequestQuery(JsonObject value) => _context["request"]!["query"] = value;

    public void SetRequestHeaders(JsonObject value) => _context["request"]!["headers"] = value;

    public void SetRequestBody(JsonNode value) => _context["request"]!["body"] = value;

    public void SetRequestRoute(JsonObject value) => _context["request"]!["route"] = value;

    public void AddAction(string key, JsonObject value) => _context["actions"]![key] = value;

    public void SetResponse(JsonObject value) => _context["response"] = value;

    public JsonObject GetResponse() => _context["response"]!.AsObject();
}
