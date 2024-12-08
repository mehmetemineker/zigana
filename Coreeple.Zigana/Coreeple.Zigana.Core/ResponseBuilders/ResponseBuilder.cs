using Coreeple.Zigana.Core.Abstractions;
using Coreeple.Zigana.Core.Json;
using Coreeple.Zigana.Core.Types;
using Json.JsonE;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Coreeple.Zigana.Core.ResponseBuilders;
public class ResponseBuilder : IResponseBuilder
{
    public async Task Build(Dictionary<string, Response> responses, JsonObject context)
    {
        var item = responses.First();

        var statusCode = item.Key;
        var response = item.Value;

        var template = JsonSerializer.SerializeToNode(response, options: SerializerOptions.DefaultJsonSerializerOptions);
        var evaluatedResponseNode = JsonE.Evaluate(template, context);
        var evaluatedResponse = JsonSerializer.Deserialize<Response>(evaluatedResponseNode)!;

        context["response"] = JsonSerializer.SerializeToNode(evaluatedResponse, options: SerializerOptions.DefaultJsonSerializerOptions);
        context["response"]!["statusCode"] = statusCode;
    }
}

