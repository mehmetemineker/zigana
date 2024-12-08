using Coreeple.Zigana.Core.Abstractions;
using Coreeple.Zigana.Core.Types.Actions;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml;

namespace Coreeple.Zigana.Core.ActionExecutors;
public class HttpRequestActionExecutor(IHttpClientFactory httpClientFactory) : IActionExecutor<HttpRequestAction>
{
    private const string HttpClientName = "ZiganaHttpClient";

    public async Task ExecuteAsync(HttpRequestAction action, CancellationToken cancellationToken)
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
        var responseDefaultHeaders = StringValuesToObject(httpResponseMessage.Headers.ToDictionary());
        var responseContentHeaders = StringValuesToObject(httpResponseMessage.Content.Headers.ToDictionary());
        var responseHeaders = responseDefaultHeaders.Union(responseContentHeaders).ToDictionary();

        var contentAsByteArray = await httpResponseMessage.Content.ReadAsByteArrayAsync(cancellationToken);
        var content = GetContent(contentAsByteArray, contentType);

        action.Output = new JsonObject
        {
            ["statusCode"] = (int)httpResponseMessage.StatusCode,
            ["headers"] = JsonSerializer.SerializeToNode(responseHeaders),
            ["content"] = new JsonObject
            {
                ["type"] = GetContentType(contentType),
                ["value"] = content
            }
        };
    }

    private static JsonNode? GetContent(byte[] contentAsByteArray, string contentType)
    {
        var contentAsString = Encoding.UTF8.GetString(contentAsByteArray);
        JsonNode? content = Convert.ToBase64String(contentAsByteArray);

        if (contentType.Contains("json"))
        {
            content = JsonNode.Parse(contentAsString)!;
        }
        else if (contentType.Contains("xml"))
        {
            XmlDocument doc = new();
            doc.LoadXml(contentAsString);

            content = JsonNode.Parse(Newtonsoft.Json.JsonConvert.SerializeXmlNode(doc))!;
        }
        else if (contentType.Contains("text"))
        {
            content = contentAsString;
        }

        return content;
    }

    private static string GetContentType(string contentType)
    {
        return contentType switch
        {
            string c when c.Contains("json") => "json",
            string c when c.Contains("xml") => "xml",
            string c when c.Contains("html") => "html",
            string c when c.Contains("text") => "text",
            string c when c.Contains("image") => "image",
            _ => "unknown"
        };
    }

    private static Dictionary<string, object> StringValuesToObject(Dictionary<string, IEnumerable<string>> values)
    {
        return values.Select(m => new KeyValuePair<string, object>(
                m.Key,
                m.Value.Count() == 1 ? m.Value.First() : m.Value.ToArray()))
            .ToDictionary();
    }
}
