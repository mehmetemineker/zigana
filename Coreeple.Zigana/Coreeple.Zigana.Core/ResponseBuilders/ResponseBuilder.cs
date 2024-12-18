using Coreeple.Zigana.Core.Abstractions;
using Coreeple.Zigana.Core.Json;
using Coreeple.Zigana.Core.Types;
using Json.JsonE;
using System.Text.Json;

namespace Coreeple.Zigana.Core.ResponseBuilders;
public class ResponseBuilder(IEndpointContext endpointContext) : IResponseBuilder
{
    public void Build(Dictionary<string, Response> responses)
    {
        foreach (var item in responses)
        {
            var statusCode = item.Key;
            var response = item.Value;

            if (JsonLogicProcessor.IsTruthy(response.When, endpointContext.Get()))
            {
                var template = JsonSerializer.SerializeToNode(response, options: CustomJsonSerializerOptions.DefaultJsonSerializerOptions);
                var evaluatedResponseNode = JsonE.Evaluate(template, endpointContext.Get());
                var evaluatedResponse = JsonSerializer.Deserialize<Response>(evaluatedResponseNode)!;

                var responseJsonObject = JsonSerializer.SerializeToNode(evaluatedResponse, options: CustomJsonSerializerOptions.DefaultJsonSerializerOptions)!.AsObject();

                responseJsonObject!["statusCode"] = statusCode;

                endpointContext.SetResponse(responseJsonObject);

                break;
            }
        }
    }
}

