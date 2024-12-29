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
          IEndpointContext endpointContext,
          IActionExecuteManager actionExecuteManager,
          IResponseBuilder responseBuilder,
          ZiganaDiagnosticSource ds)
    {
        var activity = new Activity(ZiganaDiagnosticSource.EndpointOperationName);

        try
        {
            SetHeaderDefaultResponseContentType(context);

            var endpoint = await endpointService.FindEndpointAsync(context.Request.Path, context.Request.Method, context.RequestAborted);

            SetEndpointRequestId(context, endpoint);

            StartEndpointActivity(ds, activity, endpoint);

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
            LogExceptionToActivity(activity, ex);

            throw;
        }
        finally
        {
            StopActivity(ds, activity);
        }

        if (context.Response.HasStarted)
        {
            await next(context);
        }
    }

    private static void SetEndpointRequestId(HttpContext context, EndpointDto endpoint)
    {
        if (Guid.TryParse(context.TraceIdentifier, out var requestId))
        {
            endpoint.RequestId = requestId;
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
        endpointContext.SetDefs(endpoint.Defs);
        endpointContext.SetRequestQuery(endpoint.Request.Query);
        endpointContext.SetRequestHeaders(endpoint.Request.Headers);
        endpointContext.SetRequestBody(endpoint.Request.Body);
        endpointContext.SetRequestRoute(endpoint.Request.Route);
    }

    private static void SetHeaderDefaultResponseContentType(HttpContext context) =>
        context.Response.ContentType = "application/json";


    private static Activity StartEndpointActivity(ZiganaDiagnosticSource ds, Activity activity, EndpointDto endpoint)
    {
        activity.SetStartTime(DateTime.Now);

        activity.AddTag("endpoint.id", endpoint.Id);
        activity.AddTag("request.id", endpoint.RequestId);

        return ds.StartActivity(activity, null);
    }

    private static void LogExceptionToActivity(Activity activity, Exception ex)
    {
        activity.AddException(ex, timestamp: DateTimeOffset.Now);
    }

    private static void StopActivity(DiagnosticSource diagnosticSource, Activity activity)
    {
        activity.SetEndTime(DateTime.Now);

        diagnosticSource.StopActivity(activity, null);
    }
}
