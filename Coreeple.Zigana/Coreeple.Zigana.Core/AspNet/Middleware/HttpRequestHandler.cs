using Coreeple.Zigana.Core.Abstractions;
using Coreeple.Zigana.Core.Services;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace Coreeple.Zigana.Core.AspNet.Middleware;
public class HttpRequestHandler(RequestDelegate next)
{
    public async Task InvokeAsync(
        HttpContext context,
        IEndpointService endpointService,
        IEndpointLogService endpointLogService,
        IEndpointContext endpointContext,
        IActionExecuteManager actionExecuteManager,
        IResponseBuilder responseBuilder)
    {
        SetHeaderDefaultResponseContentType(context);

        var endpoint = await endpointService.FindEndpointAsync(context, context.RequestAborted);

        endpointLogService.Add(endpoint.Id, endpoint.RequestId, "RequestStart", "SUCCEEDED");

        endpointContext.SetId(endpoint.Id);
        endpointContext.SetRequestId(endpoint.RequestId);
        endpointContext.SetDefs(endpoint.Defs);
        endpointContext.SetRequestQuery(endpoint.Request.Query);
        endpointContext.SetRequestHeaders(endpoint.Request.Headers);
        endpointContext.SetRequestBody(endpoint.Request.Body);
        endpointContext.SetRequestRoute(endpoint.Request.Route);

        try
        {
            if (endpoint.Actions != null)
            {
                await actionExecuteManager.RunAsync(endpoint.Actions, context.RequestAborted);
            }

            if (endpoint.Responses != null)
            {
                responseBuilder.Build(endpoint.Responses);

                var response = endpointContext.GetResponse();

                context.Response.StatusCode = Convert.ToInt32(response["statusCode"]!.ToString());
                await context.Response.WriteAsJsonAsync(response["content"], context.RequestAborted);
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.NoContent;
            }

            endpointLogService.Add(endpoint.Id, endpoint.RequestId, "RequestFinish", "SUCCEEDED");
        }
        catch
        {
            if (context.RequestAborted.IsCancellationRequested)
            {
                endpointLogService.Add(endpoint.Id, endpoint.RequestId, "RequestFinish", "ABORTED");
            }
            else
            {
                endpointLogService.Add(endpoint.Id, endpoint.RequestId, "RequestFinish", "FAILED");
            }

            throw;
        }

        if (context.Response.HasStarted)
        {
            await next(context);
        }
    }

    private static void SetHeaderDefaultResponseContentType(HttpContext context) =>
        context.Response.ContentType = "application/json";
}
