using Zigana.Core.JsonConverters;
using Json.JsonE;
using Json.Logic;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Zigana.Core.Helpers;
public static class JsonHelpers
{
    public static readonly JsonSerializerOptions DefaultJsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    public static readonly JsonSerializerOptions ActionConverterJsonSerializerOptions = new()
    {
        Converters = { new ActionJsonConverter() },
        PropertyNameCaseInsensitive = true,
    };
    public static object EvaluateObject(object obj, JsonObject context)
    {
        var template = JsonSerializer.SerializeToNode(obj, options: DefaultJsonSerializerOptions);
        var evaluatedNode = JsonE.Evaluate(template, context);
        var evaluated = evaluatedNode.Deserialize(obj.GetType())!;

        return evaluated;
    }

    public static bool IsTruthy(JsonNode? rule, JsonNode context)
    {
        if (rule is null) return true;

        return rule.GetValueKind() switch
        {
            JsonValueKind.Object => JsonLogic.Apply(JsonE.Evaluate(rule, context)).IsTruthy(),
            JsonValueKind.Array => rule.AsArray().All(item => IsTruthy(item, context)),
            _ => false
        };
    }
}
