using Coreeple.Zigana.Core.Abstractions;
using Coreeple.Zigana.Core.Json;
using Coreeple.Zigana.Core.Services;
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
    private readonly IEndpointLogService _endpointLogService;

    public ActionExecuteManager(IServiceProvider serviceProvider, IEndpointLogService endpointLogService)
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

        _endpointLogService = endpointLogService;
    }

    public async Task RunAsync(Endpoint endpoint, JsonObject context, CancellationToken cancellationToken = default)
    {
        var actions = endpoint.Actions ?? [];

        foreach (var (actionKey, action) in actions)
        {
            if (_executors.TryGetValue(action.GetType(), out var executor))
            {
                if (!JsonLogicProcessor.IsTruthy(action.When, context))
                {
                    _endpointLogService.Add(endpoint.Id, endpoint.RequestId, actionKey, "PASSED");
                    continue;
                }

                _endpointLogService.Add(endpoint.Id, endpoint.RequestId, actionKey, "PROCESSING");

                try
                {
                    var template = JsonSerializer.SerializeToNode(action, options: CustomJsonSerializerOptions.DefaultJsonSerializerOptions);
                    var evaluatedActionNode = JsonE.Evaluate(template, context);
                    var evaluatedAction = (Types.Action)JsonSerializer.Deserialize(evaluatedActionNode, action.GetType())!;

                    var output = await executor(evaluatedAction, cancellationToken);
                    context["actions"]![actionKey] = output;

                    _endpointLogService.Add(endpoint.Id, endpoint.RequestId, actionKey, "SUCCEEDED");
                }
                catch
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        _endpointLogService.Add(endpoint.Id, endpoint.RequestId, actionKey, "ABORTED");
                    }
                    else
                    {
                        _endpointLogService.Add(endpoint.Id, endpoint.RequestId, actionKey, "FAILED");
                    }

                    throw;
                }
            }
        }
    }
}
