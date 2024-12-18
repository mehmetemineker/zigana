using Coreeple.Zigana.Core.Abstractions;
using Coreeple.Zigana.Core.Json;
using Coreeple.Zigana.Core.Services;
using Coreeple.Zigana.Core.Types.Actions;
using Coreeple.Zigana.Core.Utils;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Coreeple.Zigana.Core.ActionExecutors;
public class ActionExecuteManager : IActionExecuteManager
{
    private readonly Dictionary<Type, Func<Types.Action, CancellationToken, Task<JsonNode?>>> _executors;
    private readonly IServiceProvider _serviceProvider;
    private readonly IEndpointContext _endpointContext;

    public ActionExecuteManager(IServiceProvider serviceProvider, IEndpointContext endpointContext)
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

        _serviceProvider = serviceProvider;
        _endpointContext = endpointContext;
    }

    public async Task RunAsync(Dictionary<string, Types.Action> actions, CancellationToken cancellationToken = default)
    {
        var endpointLogService = _serviceProvider.GetRequiredService<IEndpointLogService>();

        foreach (var (actionKey, action) in actions)
        {
            if (_executors.TryGetValue(action.GetType(), out var executor))
            {
                if (!JsonLogicProcessor.IsTruthy(action.When, _endpointContext.Get()))
                {
                    endpointLogService.AddTransaction(_endpointContext.GetId(), _endpointContext.GetRequestId(), actionKey, "PASSED");
                    continue;
                }

                endpointLogService.AddTransaction(_endpointContext.GetId(), _endpointContext.GetRequestId(), actionKey, "PROCESSING");

                try
                {
                    if (action is ParallelAction)
                    {
                        await executor(action, cancellationToken);
                    }
                    else
                    {
                        var evaluatedAction = (Types.Action)JsonUtils.EvaluateObject(action, _endpointContext.Get());

                        var output = await executor(evaluatedAction, cancellationToken);
                        _endpointContext.AddAction(actionKey, output!.AsObject());

                        endpointLogService.AddLog("Info", JsonSerializer.Serialize(evaluatedAction));
                    }

                    endpointLogService.AddTransaction(_endpointContext.GetId(), _endpointContext.GetRequestId(), actionKey, "SUCCEEDED");
                }
                catch (Exception ex)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        endpointLogService.AddTransaction(_endpointContext.GetId(), _endpointContext.GetRequestId(), actionKey, "ABORTED");
                    }
                    else
                    {
                        endpointLogService.AddTransaction(_endpointContext.GetId(), _endpointContext.GetRequestId(), actionKey, "FAILED");
                    }

                    endpointLogService.AddLog("Error", ex.Message);

                    throw;
                }
            }
        }
    }
}
