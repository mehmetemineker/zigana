using Coreeple.Zigana.Core.Helpers;
using Coreeple.Zigana.EndpointProcessor.Abstractions;
using Coreeple.Zigana.Services.Abstractions;
using Coreeple.Zigana.Services.Dtos;
using Microsoft.AspNetCore.Http;
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

        string path = context.Request.Path;
        string method = context.Request.Method;

        var endpoint = await endpointService.FindEndpointAsync(path, method, context.RequestAborted);

        await endpointLogService.AddTransactionAsync(new EndpointTransactionCreateDto()
        {
            EndpointId = endpoint.Id,
            RequestId = endpoint.RequestId,
            Name = "RequestStart",
            Status = "SUCCEEDED",
        });

        await SetEndpointRequestFromHttpContext(context, endpoint, context.RequestAborted);
        FillEndpointContext(endpointContext, endpoint);

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

            await endpointLogService.AddTransactionAsync(new EndpointTransactionCreateDto()
            {
                EndpointId = endpoint.Id,
                RequestId = endpoint.RequestId,
                Name = "RequestFinish",
                Status = "SUCCEEDED",
            });
        }
        catch
        {
            if (context.RequestAborted.IsCancellationRequested)
            {
                await endpointLogService.AddTransactionAsync(new EndpointTransactionCreateDto()
                {
                    EndpointId = endpoint.Id,
                    RequestId = endpoint.RequestId,
                    Name = "RequestFinish",
                    Status = "ABORTED",
                });
            }
            else
            {
                await endpointLogService.AddTransactionAsync(new EndpointTransactionCreateDto()
                {
                    EndpointId = endpoint.Id,
                    RequestId = endpoint.RequestId,
                    Name = "RequestFinish",
                    Status = "FAILED",
                });
            }

            throw;
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

        if (Guid.TryParse(context.TraceIdentifier, out var requestId))
        {
            endpoint.RequestId = requestId;
        }
    }

    private static void FillEndpointContext(IEndpointContext endpointContext, EndpointDto endpoint)
    {
        endpointContext.SetId(endpoint.Id);
        endpointContext.SetRequestId(endpoint.RequestId);
        endpointContext.SetDefs(endpoint.Defs);
        endpointContext.SetRequestQuery(endpoint.Request.Query);
        endpointContext.SetRequestHeaders(endpoint.Request.Headers);
        endpointContext.SetRequestBody(endpoint.Request.Body);
        endpointContext.SetRequestRoute(endpoint.Request.Route);
    }

    private static void SetHeaderDefaultResponseContentType(HttpContext context) =>
        context.Response.ContentType = "application/json";
}
