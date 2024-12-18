using Coreeple.Zigana.Core.Abstractions;
using Coreeple.Zigana.Core.Types.Actions;
using HtmlAgilityPack;
using System.Text.Json.Nodes;

namespace Coreeple.Zigana.Core.ActionExecutors;
public class HtmlParserActionExecutor : IActionExecutor<HtmlParserAction>
{
    public async Task<JsonNode?> ExecuteAsync(HtmlParserAction action, CancellationToken cancellationToken)
    {
        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(action.Source);

        var nodes = new JsonArray();

        foreach (var selector in action.Selectors)
        {
            var values = selector.Value.Split(',');
            var htmlNodes = htmlDoc.DocumentNode.SelectNodes(selector.Key);

            if (htmlNodes is null)
            {
                continue;
            }

            foreach (var htmlNode in htmlNodes)
            {
                var attributes = htmlNode.Attributes.ToDictionary(attr => attr.Name, attr => attr.Value);

                if (!string.IsNullOrWhiteSpace(htmlNode.InnerHtml))
                {
                    attributes.Add("innerHtml", htmlNode.InnerHtml);
                }

                var node = new JsonObject();

                if (values.Contains("*"))
                {
                    foreach (var attribute in attributes)
                    {
                        node[attribute.Key] = attribute.Value;
                    }
                }
                else
                {
                    foreach (var value in values)
                    {
                        node[value] = null;

                        if (attributes.TryGetValue(value, out string? attrValue))
                        {
                            node[value] = attrValue;
                        }
                    }
                }

                if (node.Count > 0)
                {
                    nodes.Add(node);
                }
            }
        }

        return await Task.FromResult<JsonNode?>(new JsonObject()
        {
            ["nodes"] = nodes
        });
    }
}
