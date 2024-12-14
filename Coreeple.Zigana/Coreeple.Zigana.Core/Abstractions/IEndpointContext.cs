using System.Text.Json.Nodes;

namespace Coreeple.Zigana.Core.Abstractions;
public interface IEndpointContext
{
    void SetId(Guid value);
    void SetRequestId(Guid value);
    void SetDefs(JsonObject value);
    void SetRequestQuery(JsonObject value);
    void SetRequestHeaders(JsonObject value);
    void SetRequestBody(JsonNode value);
    void SetRequestRoute(JsonObject value);
    void AddAction(string key, JsonObject value);
    void SetResponse(JsonObject value);
    JsonObject Get();
    Guid GetId();
    Guid GetRequestId();
    JsonObject GetResponse();
}
