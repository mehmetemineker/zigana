using Json.JsonE;
using Json.Logic;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Coreeple.Zigana.Core.Json;
public class JsonLogicProcessor
{
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
