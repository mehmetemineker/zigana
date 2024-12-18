using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.Extensions.Primitives;
using System.Text;
using System.Text.Json.Nodes;
using System.Xml;

namespace Coreeple.Zigana.Core.Utils;
public class HttpUtils
{
    public static Dictionary<string, object> StringValuesToObject(Dictionary<string, List<string>> values)
    {
        return values.Select(m => new KeyValuePair<string, object>(
                m.Key,
                m.Value.Count == 1 ? m.Value.First() : m.Value.ToArray()))
            .ToDictionary();
    }

    public static Dictionary<string, object> StringValuesToObject(Dictionary<string, StringValues> values)
    {
        return StringValuesToObject(values.ToDictionary(m => m.Key, m => m.Value.Select(m => m!).ToList()));
    }

    public static Dictionary<string, object> StringValuesToObject(Dictionary<string, IEnumerable<string>> values)
    {
        return StringValuesToObject(values);
    }

    public static string GetContentType(string contentType)
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

    public static JsonNode? GetContent(byte[] contentAsByteArray, string contentType)
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

    public static bool TryGetMatchingEndpoint(string path, HashSet<string> endpoints, out (string Pattern, Dictionary<string, object?> Parameters) matchingEndpoint)
    {
        var values = new RouteValueDictionary();

        foreach (var endpoint in endpoints)
        {
            var template = TemplateParser.Parse(endpoint.Split(':')[0]);
            var matcher = new TemplateMatcher(template, values);

            if (matcher.TryMatch(path, values))
            {
                matchingEndpoint = (endpoint, values.ToDictionary());

                return true;
            }
        }

        matchingEndpoint = (string.Empty, new Dictionary<string, object?>());

        return false;
    }
}
