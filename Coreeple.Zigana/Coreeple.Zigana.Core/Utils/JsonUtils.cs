using Coreeple.Zigana.Core.Json;
using Coreeple.Zigana.Core.Types;
using Json.JsonE;
using System.Text.Json;
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

    public static object EvaluateObject(object obj, JsonObject context)
    {
        var template = JsonSerializer.SerializeToNode(obj, options: CustomJsonSerializerOptions.DefaultJsonSerializerOptions);
        var evaluatedNode = JsonE.Evaluate(template, context);
        var evaluated = JsonSerializer.Deserialize(evaluatedNode, obj.GetType())!;

        return evaluated;
    }
}
