using Coreeple.Zigana.Core.Abstractions;
using Coreeple.Zigana.Core.Types.Actions;
using System.Text.Json.Nodes;

namespace Coreeple.Zigana.Core.ActionExecutors;
public class ParallelActionExecutor(IActionExecuteManager actionExecuteManager) : IActionExecutor<ParallelAction>
{
    public async Task<JsonNode?> ExecuteAsync(ParallelAction action, CancellationToken cancellationToken)
    {
        var tasks = new List<Task>();

        foreach (var subAction in action.Actions)
        {
            tasks.Add(actionExecuteManager.RunAsync(new Dictionary<string, Types.Action> { { subAction.Key, subAction.Value } }, cancellationToken));
        }

        if (action.Continue == "when-all")
        {
            await Task.WhenAll(tasks);
        }
        else if (action.Continue == "when-any")
        {
            await Task.WhenAny(tasks);
        }

        return default;
    }
}
