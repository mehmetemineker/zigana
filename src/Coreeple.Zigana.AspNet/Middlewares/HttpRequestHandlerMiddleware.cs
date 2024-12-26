using Coreeple.Zigana.Core.Diagnostics;
using Coreeple.Zigana.Core.Helpers;
using Coreeple.Zigana.EndpointProcessor.Abstractions;
using Coreeple.Zigana.Services.Abstractions;
using Coreeple.Zigana.Services.Dtos;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Coreeple.Zigana.AspNet.Middlewares;
public class HttpRequestHandlerMiddleware(RequestDelegate next)
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

        var activity = new Activity("HttpRequestHandler");

        ZiganaDiagnosticSource.Instance.StartActivity(activity, null);

        context.TraceIdentifier = activity.Id!;
        context.Response.Headers["X-Request-Id"] = context.TraceIdentifier;

        var endpoint = await endpointService.FindEndpointAsync(context.Request.Path, context.Request.Method, context.RequestAborted);

        try
        {
            await SetEndpointRequestFromHttpContext(context, endpoint, context.RequestAborted);
            FillEndpointContext(endpointContext, endpoint);

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
        }
        catch (Exception ex)
        {
            activity.AddException(ex);
            throw;
        }
        finally
        {
            ZiganaDiagnosticSource.Instance.StopActivity(activity, null);
        }


        if (context.Response.HasStarted)
        {
            await next(context);
        }
    }

    private static async Task SetEndpointRequestFromHttpContext(HttpContext context, EndpointDto endpoint, CancellationToken cancellationToken)
    {
        var query = HttpHelpers.StringValuesToObject(context.Request.Query.ToDictionary());
        var headers = HttpHelpers.StringValuesToObject(context.Request.Headers.ToDictionary());
        var body = await new StreamReader(context.Request.Body, Encoding.UTF8).ReadToEndAsync(cancellationToken);

        endpoint.Request.Query = JsonNode.Parse(JsonSerializer.Serialize(query))?.AsObject() ?? [];
        endpoint.Request.Headers = JsonNode.Parse(JsonSerializer.Serialize(headers))?.AsObject() ?? [];

        if (context.Request.ContentType == "application/json" && !string.IsNullOrEmpty(body))
        {
            endpoint.Request.Body = JsonNode.Parse(body) ?? JsonNode.Parse("{}")!;
        }
    }

    private static void FillEndpointContext(IEndpointContext endpointContext, EndpointDto endpoint)
    {
        endpointContext.SetId(endpoint.Id);
        endpointContext.SetDefs(endpoint.Defs);
        endpointContext.SetRequestQuery(endpoint.Request.Query);
        endpointContext.SetRequestHeaders(endpoint.Request.Headers);
        endpointContext.SetRequestBody(endpoint.Request.Body);
        endpointContext.SetRequestRoute(endpoint.Request.Route);
    }

    private static void SetHeaderDefaultResponseContentType(HttpContext context) =>
        context.Response.ContentType = "application/json";
}
