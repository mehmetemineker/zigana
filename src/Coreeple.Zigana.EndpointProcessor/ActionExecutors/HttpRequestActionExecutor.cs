using Coreeple.Zigana.Core.Helpers;
using Coreeple.Zigana.Core.Types;
using Coreeple.Zigana.EndpointProcessor.Abstractions;
using Microsoft.AspNetCore.WebUtilities;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Coreeple.Zigana.EndpointProcessor.ActionExecutors;
public class HttpRequestActionExecutor(IHttpClientFactory httpClientFactory) : IActionExecutor<HttpRequestAction>
{
    private const string HttpClientName = "ZiganaHttpClient";

    public async Task<JsonNode?> ExecuteAsync(HttpRequestAction action, CancellationToken cancellationToken)
    {
        string url = QueryHelpers.AddQueryString(action.Url, action.Query);

        var httpRequestMessage = new HttpRequestMessage(new HttpMethod(action.Method), url);
        var httpClient = httpClientFactory.CreateClient(HttpClientName);

        SetRequestContent(action, httpRequestMessage);
        SetDefaultRequestHeaders(action, httpClient);

        var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage, cancellationToken);

        var responseHeaders = GetResponseHeaders(httpResponseMessage);
        var contentAsByteArray = await httpResponseMessage.Content.ReadAsByteArrayAsync(cancellationToken);
        var contentType = string.Join(' ', httpResponseMessage.Content.Headers.GetValues("Content-Type"));
        var content = HttpHelpers.GetContent(contentAsByteArray, contentType);

        return new JsonObject
        {
            ["statusCode"] = (int)httpResponseMessage.StatusCode,
            ["headers"] = JsonSerializer.SerializeToNode(responseHeaders),
            ["content"] = new JsonObject
            {
                ["type"] = HttpHelpers.GetContentType(contentType),
                ["value"] = content
            }
        };
    }

    private static Dictionary<string, object> GetResponseHeaders(HttpResponseMessage httpResponseMessage)
    {
        var responseDefaultHeaders = HttpHelpers.StringValuesToObject(httpResponseMessage.Headers.ToDictionary());
        var responseContentHeaders = HttpHelpers.StringValuesToObject(httpResponseMessage.Content.Headers.ToDictionary());
        var responseHeaders = responseDefaultHeaders.Union(responseContentHeaders).ToDictionary();
        return responseHeaders;
    }

    private static void SetRequestContent(HttpRequestAction action, HttpRequestMessage httpRequestMessage)
    {
        if (action.Body is not null && action.Headers.TryGetValue("Content-Type", out var requestContentType))
        {
            httpRequestMessage.Content = requestContentType switch
            {
                "application/x-www-form-urlencoded" or "multipart/form-data" =>
                    new FormUrlEncodedContent(JsonSerializer.Deserialize<Dictionary<string, string>>(action.Body)!),
                "application/json" =>
                    JsonContent.Create(action.Body),
                _ => null
            };
        }
    }

    private static void SetDefaultRequestHeaders(HttpRequestAction action, HttpClient httpClient)
    {
        httpClient.DefaultRequestHeaders.Clear();

        foreach (var header in action.Headers)
        {
            if (!httpClient.DefaultRequestHeaders.Contains(header.Key))
            {
                httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
            }
        }
    }
}
