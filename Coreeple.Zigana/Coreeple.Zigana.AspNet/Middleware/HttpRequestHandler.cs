using Coreeple.Zigana.Core.Abstractions;
using Coreeple.Zigana.Core.Services;
using Coreeple.Zigana.Core.Utils;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace Coreeple.Zigana.AspNet.Middleware;
public class HttpRequestHandler(
    IEndpointService endpointService,
    ILogService logService,
    IActionExecuteManager actionExecuteManager,
    IResponseBuilder responseBuilder)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var requestId = Guid.NewGuid();

        SetHeaderDefaultResponseContentType(context);
        SetHeaderRequestId(context, requestId);

        var endpoint = await endpointService.FindEndpointAsync(context, context.RequestAborted);

        await logService.BeginAsync(requestId, endpoint.Id, context.RequestAborted);

        var endpointContext = JsonUtils.CreateEndpointContext(endpoint);

        if (endpoint.Actions != null)
        {
            await actionExecuteManager.RunAsync(endpoint.Actions, endpointContext, context.RequestAborted);
        }

        if (endpoint.Responses != null)
        {
            responseBuilder.Build(endpoint.Responses, endpointContext);

            var response = endpointContext["response"]!;

            context.Response.StatusCode = Convert.ToInt32(response["statusCode"]!.ToString());
            await context.Response.WriteAsJsonAsync(response["content"], context.RequestAborted);
        }
        else
        {
            context.Response.StatusCode = (int)HttpStatusCode.NoContent;
        }

        await logService.EndAsync(requestId, context.RequestAborted);
    }

    private static void SetHeaderRequestId(HttpContext context, Guid requestId)
    {
        const string headerKey = "X-Request-Id";
        context.Request.Headers[headerKey] = requestId.ToString();
        context.Response.Headers[headerKey] = requestId.ToString();
    }

    private static void SetHeaderDefaultResponseContentType(HttpContext context) =>
        context.Response.ContentType = "application/json";
}
