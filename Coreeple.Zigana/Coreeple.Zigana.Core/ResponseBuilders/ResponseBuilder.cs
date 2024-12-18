using Coreeple.Zigana.Core.Abstractions;
using Coreeple.Zigana.Core.Json;
using Coreeple.Zigana.Core.Types;
using Coreeple.Zigana.Core.Utils;
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
                var evaluatedResponse = (Response)JsonUtils.EvaluateObject(response, endpointContext.Get());

                var responseJsonObject = JsonSerializer.SerializeToNode(evaluatedResponse, options: CustomJsonSerializerOptions.DefaultJsonSerializerOptions)!.AsObject();

                responseJsonObject!["statusCode"] = statusCode;

                endpointContext.SetResponse(responseJsonObject);

                break;
            }
        }
    }
}

