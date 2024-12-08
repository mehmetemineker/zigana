using Coreeple.Zigana.Core.Types;
using System.Text.Json.Nodes;

namespace Coreeple.Zigana.Core.Utils;
public class JsonUtils
{
    public static JsonObject CreateEndpointContext(Endpoint endpoint)
    {
        return new JsonObject
        {
            ["defs"] = endpoint.Defs,
            ["request"] = new JsonObject()
            {
                ["query"] = endpoint.Request?.Query,
                ["headers"] = endpoint.Request?.Headers,
                ["body"] = endpoint.Request?.Body,
                ["route"] = endpoint.Request?.Route
            },
            ["actions"] = new JsonObject(),
            ["response"] = new JsonObject()
        };
    }
}
