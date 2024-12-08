using Coreeple.Zigana.Core.Abstractions;
using Coreeple.Zigana.Core.Json;
using Coreeple.Zigana.Core.Types;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Coreeple.Zigana.Core.ResponseBuilders;
public class ResponseBuilder : IResponseBuilder
{
    public async Task Build(Dictionary<string, Response> response, JsonObject context)
    {
        var item = response.First();

        context["response"] = JsonSerializer.SerializeToNode(item.Value, options: SerializerOptions.DefaultJsonSerializerOptions);
        context["response"]!["statusCode"] = item.Key;
    }
}

