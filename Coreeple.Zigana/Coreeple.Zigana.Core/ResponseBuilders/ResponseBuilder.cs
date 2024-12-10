using Coreeple.Zigana.Core.Abstractions;
using Coreeple.Zigana.Core.Json;
using Coreeple.Zigana.Core.Types;
using Json.JsonE;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Coreeple.Zigana.Core.ResponseBuilders;
public class ResponseBuilder : IResponseBuilder
{
    public void Build(Dictionary<string, Response> responses, JsonObject context)
    {
        foreach (var item in responses)
        {
            var statusCode = item.Key;
            var response = item.Value;

            if (JsonLogicProcessor.IsTruthy(response.When, context))
            {
                var template = JsonSerializer.SerializeToNode(response, options: CustomJsonSerializerOptions.DefaultJsonSerializerOptions);
                var evaluatedResponseNode = JsonE.Evaluate(template, context);
                var evaluatedResponse = JsonSerializer.Deserialize<Response>(evaluatedResponseNode)!;

                context["response"] = JsonSerializer.SerializeToNode(evaluatedResponse, options: CustomJsonSerializerOptions.DefaultJsonSerializerOptions);
                context["response"]!["statusCode"] = statusCode;

                return;
            }
        }
    }
}

