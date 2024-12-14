using Coreeple.Zigana.Core.Abstractions;
using Coreeple.Zigana.Core.Json;
using Coreeple.Zigana.Core.Services;
using Coreeple.Zigana.Core.Types.Actions;
using Json.JsonE;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Coreeple.Zigana.Core.ActionExecutors;
public class ActionExecuteManager : IActionExecuteManager
{
    private readonly Dictionary<Type, Func<Types.Action, CancellationToken, Task<JsonNode?>>> _executors;
    private readonly IEndpointContext _endpointContext;
    private readonly IEndpointLogService _endpointLogService;

    public ActionExecuteManager(IServiceProvider serviceProvider, IEndpointContext endpointContext, IEndpointLogService endpointLogService)
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
                    return await executor.ExecuteAsync((ParallelAction)action, cancellationToken);
                }
            },
            {
                typeof(HtmlParserAction), async (action, cancellationToken) => {
                    var executor = serviceProvider.GetRequiredService<IActionExecutor<HtmlParserAction>>();
                    return await executor.ExecuteAsync((HtmlParserAction)action, cancellationToken);
                }
            },
        };

        _endpointContext = endpointContext;
        _endpointLogService = endpointLogService;
    }

    public async Task RunAsync(Dictionary<string, Types.Action> actions, CancellationToken cancellationToken = default)
    {
        foreach (var (actionKey, action) in actions)
        {
            if (_executors.TryGetValue(action.GetType(), out var executor))
            {
                if (!JsonLogicProcessor.IsTruthy(action.When, _endpointContext.Get()))
                {
                    _endpointLogService.Add(_endpointContext.GetId(), _endpointContext.GetRequestId(), actionKey, "PASSED");
                    continue;
                }

                _endpointLogService.Add(_endpointContext.GetId(), _endpointContext.GetRequestId(), actionKey, "PROCESSING");

                try
                {
                    if (action is ParallelAction)
                    {
                        await executor(action, cancellationToken);
                    }
                    else
                    {
                        var template = JsonSerializer.SerializeToNode(action, options: CustomJsonSerializerOptions.DefaultJsonSerializerOptions);
                        var evaluatedActionNode = JsonE.Evaluate(template, _endpointContext.Get());
                        var evaluatedAction = (Types.Action)JsonSerializer.Deserialize(evaluatedActionNode, action.GetType())!;

                        var output = await executor(evaluatedAction, cancellationToken);
                        _endpointContext.AddAction(actionKey, output!.AsObject());
                    }

                    _endpointLogService.Add(_endpointContext.GetId(), _endpointContext.GetRequestId(), actionKey, "SUCCEEDED");
                }
                catch
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        _endpointLogService.Add(_endpointContext.GetId(), _endpointContext.GetRequestId(), actionKey, "ABORTED");
                    }
                    else
                    {
                        _endpointLogService.Add(_endpointContext.GetId(), _endpointContext.GetRequestId(), actionKey, "FAILED");
                    }

                    throw;
                }
            }
        }
    }
}
