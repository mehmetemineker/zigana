﻿using Zigana.Core.Types;
using Zigana.EndpointProcessor.Abstractions;
using System.Text.Json.Nodes;
using Action = Zigana.Core.Types.Action;

namespace Zigana.EndpointProcessor.ActionExecutors;
public class ParallelActionExecutor(IActionExecuteManager actionExecuteManager) : IActionExecutor<ParallelAction>
{
    public async Task<JsonNode?> ExecuteAsync(ParallelAction action, CancellationToken cancellationToken)
    {
        var tasks = new List<Task>();

        foreach (var subAction in action.Actions)
        {
            tasks.Add(actionExecuteManager.RunAsync(
                new Dictionary<string, Action> { { subAction.Key, subAction.Value } }, cancellationToken));
        }

        try
        {
            await Task.WhenAll(tasks);
        }
        catch (Exception)
        {
            // TODO: diaper
        }

        return default;
    }
}
