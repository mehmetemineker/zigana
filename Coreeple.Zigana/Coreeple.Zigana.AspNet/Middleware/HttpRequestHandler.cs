using Coreeple.Zigana.Core.Abstractions;
using Coreeple.Zigana.Core.Services;
using Coreeple.Zigana.Core.Utils;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace Coreeple.Zigana.AspNet.Middleware;
public class HttpRequestHandler(
    RequestDelegate next,
    IEndpointService endpointService,
    IEndpointLogService endpointLogService,
    IActionExecuteManager actionExecuteManager,
    IResponseBuilder responseBuilder)
{
    public async Task InvokeAsync(HttpContext context)
    {
        SetHeaderDefaultResponseContentType(context);

        var endpoint = await endpointService.FindEndpointAsync(context, context.RequestAborted);

        SetHeaderRequestId(context, endpoint.RequestId);

        await endpointLogService.AddAsync(endpoint.Id, endpoint.RequestId, "RequestStart", "SUCCEEDED");

        var endpointContext = JsonUtils.CreateEndpointContext(endpoint);

        try
        {
            await actionExecuteManager.RunAsync(endpoint, endpointContext, context.RequestAborted);

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

            await endpointLogService.AddAsync(endpoint.Id, endpoint.RequestId, "RequestFinish", "SUCCEEDED");
        }
        catch
        {
            if (context.RequestAborted.IsCancellationRequested)
            {
                await endpointLogService.AddAsync(endpoint.Id, endpoint.RequestId, "RequestFinish", "ABORTED");
            }
            else
            {
                await endpointLogService.AddAsync(endpoint.Id, endpoint.RequestId, "RequestFinish", "FAILED");
            }

            throw;
        }

        if (context.Response.HasStarted)
        {
            await next(context);
        }
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
