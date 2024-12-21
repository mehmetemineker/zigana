using Coreeple.Zigana.Core.Helpers;
using Coreeple.Zigana.Core.Types;
using Coreeple.Zigana.EndpointProcessor.Abstractions;
using System.Text.Json;

namespace Coreeple.Zigana.EndpointProcessor.ResponseBuilders;
public class ResponseBuilder(IEndpointContext endpointContext) : IResponseBuilder
{
    public void Build(Dictionary<string, Response> responses)
    {
        foreach (var item in responses)
        {
            var statusCode = item.Key;
            var response = item.Value;

            if (JsonHelpers.IsTruthy(response.When, endpointContext.Get()))
            {
                var evaluatedResponse = (Response)JsonHelpers.EvaluateObject(response, endpointContext.Get());

                var responseJsonObject = JsonSerializer.SerializeToNode(evaluatedResponse, options: JsonHelpers.DefaultJsonSerializerOptions)!.AsObject();

                responseJsonObject!["statusCode"] = statusCode;

                endpointContext.SetResponse(responseJsonObject);

                break;
            }
        }
    }
}

