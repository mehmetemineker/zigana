using Coreeple.Zigana.Core.Abstractions;
using Coreeple.Zigana.Core.Types.Actions;
using Coreeple.Zigana.Core.Utils;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Coreeple.Zigana.Core.ActionExecutors;
public class HttpRequestActionExecutor(IHttpClientFactory httpClientFactory) : IActionExecutor<HttpRequestAction>
{
    private const string HttpClientName = "ZiganaHttpClient";

    public async Task<JsonNode?> ExecuteAsync(HttpRequestAction action, CancellationToken cancellationToken)
    {
        var httpRequestMessage = new HttpRequestMessage(new HttpMethod(action.Method), action.Url);
        var headers = JsonSerializer.Deserialize<Dictionary<string, string>>(action.Headers)!;

        if (action.Body is not null && headers.TryGetValue("Content-Type", out var requestContentType))
        {
            if (requestContentType == "application/x-www-form-urlencoded" || requestContentType == "multipart/form-data")
            {
                var body = JsonSerializer.Deserialize<Dictionary<string, string>>(action.Body)!;
                httpRequestMessage.Content = new FormUrlEncodedContent(body);
            }
            else if (requestContentType == "application/json")
            {
                httpRequestMessage.Content = JsonContent.Create(action.Body);
            }

            headers.Remove("Content-Type");
        }

        var httpClient = httpClientFactory.CreateClient(HttpClientName);

        httpClient.DefaultRequestHeaders.Clear();

        foreach (var header in headers)
        {
            httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
        }

        var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage, cancellationToken);

        var contentType = string.Join(' ', httpResponseMessage.Content.Headers.GetValues("Content-Type"));

        var responseDefaultHeaders = HttpUtils.StringValuesToObject(httpResponseMessage.Headers.ToDictionary());
        var responseContentHeaders = HttpUtils.StringValuesToObject(httpResponseMessage.Content.Headers.ToDictionary());
        var responseHeaders = responseDefaultHeaders.Union(responseContentHeaders).ToDictionary();

        var contentAsByteArray = await httpResponseMessage.Content.ReadAsByteArrayAsync(cancellationToken);
        var content = HttpUtils.GetContent(contentAsByteArray, contentType);

        return new JsonObject
        {
            ["statusCode"] = (int)httpResponseMessage.StatusCode,
            ["headers"] = JsonSerializer.SerializeToNode(responseHeaders),
            ["content"] = new JsonObject
            {
                ["type"] = HttpUtils.GetContentType(contentType),
                ["value"] = content
            }
        };
    }
}
