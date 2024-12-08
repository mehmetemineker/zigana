using Coreeple.Zigana.Core.Abstractions;
using Coreeple.Zigana.Core.Json;
using Coreeple.Zigana.Core.Types;
using Coreeple.Zigana.Core.Types.Actions;
using Json.JsonE;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Coreeple.Zigana.Core.ActionExecutors;
public class ActionExecuteManager : IActionExecuteManager
{
    private readonly Dictionary<Type, Func<Types.Action, CancellationToken, Task<JsonNode?>>> _executors;

    public ActionExecuteManager(IServiceProvider serviceProvider)
    {
        _executors = new()
        {
            {
                typeof(HttpRequestAction), async (action, cancellationToken) => {
                    var executor = serviceProvider.GetRequiredService<IActionExecutor<HttpRequestAction>>();
                    return await executor.ExecuteAsync((HttpRequestAction)action, cancellationToken);
                }
            },
            {
                typeof(ParallelAction), async (action, cancellationToken) => {
                    var executor = serviceProvider.GetRequiredService<IActionExecutor<ParallelAction>>();
                    return  await executor.ExecuteAsync((ParallelAction)action, cancellationToken);
                }
            }
        };
    }

    public async Task<JsonObject> RunAsync(Endpoint endpoint, CancellationToken cancellationToken = default)
    {
        if (endpoint.Actions == null)
        {
            return [];
        }

        var context = new JsonObject
        {
            ["defs"] = endpoint.Defs,
            ["request"] = new JsonObject()
            {
                ["query"] = endpoint.Request?.Query,
                ["headers"] = endpoint.Request?.Headers,
                ["body"] = endpoint.Request?.Body,
                ["route"] = endpoint.Request?.Route
            },
            ["actions"] = new JsonObject()
        };

        foreach (var (actionKey, action) in endpoint.Actions)
        {
            if (action == null)
            {
                continue;
            }

            if (_executors.TryGetValue(action.GetType(), out var executor))
            {
                var template = JsonSerializer.SerializeToNode(action, options: SerializerOptions.DefaultJsonSerializerOptions);
                var evaluatedActionNode = JsonE.Evaluate(template, context);
                var evaluatedAction = (Types.Action)JsonSerializer.Deserialize(evaluatedActionNode, action.GetType())!;

                var output = await executor(evaluatedAction, cancellationToken);

                context["actions"]![actionKey] = output;
            }
        }

        return context["actions"]!.AsObject();
    }
}
