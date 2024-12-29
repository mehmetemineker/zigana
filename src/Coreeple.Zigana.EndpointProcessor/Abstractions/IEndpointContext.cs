using System.Text.Json.Nodes;

namespace Coreeple.Zigana.EndpointProcessor.Abstractions;
public interface IEndpointContext
{
    void SetDefs(JsonObject value);
    void SetRequestQuery(JsonObject value);
    void SetRequestHeaders(JsonObject value);
    void SetRequestBody(JsonNode value);
    void SetRequestRoute(JsonObject value);
    void AddAction(string key, JsonObject value);
    void SetResponse(JsonObject value);
    JsonObject Get();
    JsonObject GetResponse();
}
