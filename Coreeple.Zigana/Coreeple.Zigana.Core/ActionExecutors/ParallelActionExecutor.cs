using Coreeple.Zigana.Core.Abstractions;
using Coreeple.Zigana.Core.Json;
using Coreeple.Zigana.Core.Types.Actions;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Coreeple.Zigana.Core.ActionExecutors;
public class ParallelActionExecutor(IActionExecuteManager actionExecuteManager) : IActionExecutor<ParallelAction>
{
    public async Task<JsonNode?> ExecuteAsync(ParallelAction action, CancellationToken cancellationToken)
    {
        var tasks = new List<Task<KeyValuePair<string, JsonNode>>>();

        //foreach (var subAction in action.Actions)
        //{
        //    tasks.Add(Task.Run(async () =>
        //    {
        //        var output = await actionExecuteManager.RunAsync(subAction.Value);
        //        return new KeyValuePair<string, JsonNode>(subAction.Key, output!);
        //    }));
        //}

        if (action.Continue == "when-all")
        {
            var results = await Task.WhenAll(tasks);
            var dictionary = results.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            return JsonSerializer.SerializeToNode(dictionary, options: CustomJsonSerializerOptions.DefaultJsonSerializerOptions);
        }
        else if (action.Continue == "when-any")
        {
            var result = await Task.WhenAny(tasks);
            var kvp = result.Result;

            return kvp.Value;
        }

        return default;
    }
}
