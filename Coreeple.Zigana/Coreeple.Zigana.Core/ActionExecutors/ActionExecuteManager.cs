using Coreeple.Zigana.Core.Abstractions;
using Coreeple.Zigana.Core.Json;
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

    public async Task RunAsync(Dictionary<string, Types.Action> actions, JsonObject context, CancellationToken cancellationToken = default)
    {
        foreach (var (actionKey, action) in actions)
        {
            if (_executors.TryGetValue(action.GetType(), out var executor))
            {
                var template = JsonSerializer.SerializeToNode(action, options: SerializerOptions.DefaultJsonSerializerOptions);
                var evaluatedActionNode = JsonE.Evaluate(template, context);
                var evaluatedAction = (Types.Action)JsonSerializer.Deserialize(evaluatedActionNode, action.GetType())!;

                var output = await executor(evaluatedAction, cancellationToken);

                context["actions"]![actionKey] = output;
            }
        }
    }
}
