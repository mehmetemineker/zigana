using Coreeple.Zigana.Core.Diagnostics;
using Coreeple.Zigana.Core.Helpers;
using Coreeple.Zigana.Core.Types;
using Coreeple.Zigana.EndpointProcessor.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Text.Json.Nodes;
using Action = Coreeple.Zigana.Core.Types.Action;

namespace Coreeple.Zigana.EndpointProcessor.ActionExecutors;
public class ActionExecuteManager : IActionExecuteManager
{
    private readonly Dictionary<Type, Func<Action, CancellationToken, Task<JsonNode?>>> _executors;
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

    public async Task RunAsync(Dictionary<string, Action> actions, CancellationToken cancellationToken = default)
    {
        foreach (var (actionKey, action) in actions)
        {
            var activity = new Activity("ActionExecuteManager")
            {
                DisplayName = actionKey
            };

            ZiganaDiagnosticSource.Instance.StartActivity(activity, null);

            try
            {
                if (_executors.TryGetValue(action.GetType(), out var executor))
                {
                    if (!JsonHelpers.IsTruthy(action.When, _endpointContext.Get()))
                    {
                        activity.AddEvent(new ActivityEvent("Skipped", DateTimeOffset.Now));
                        continue;
                    }

                    activity.AddEvent(new ActivityEvent("Started", DateTimeOffset.Now));


                    if (action is ParallelAction)
                    {
                        await executor(action, cancellationToken);
                    }
                    else
                    {
                        var evaluatedAction = (Action)JsonHelpers.EvaluateObject(action, _endpointContext.Get());

                        var output = await executor(evaluatedAction, cancellationToken);
                        _endpointContext.AddAction(actionKey, output!.AsObject());
                    }

                    activity.AddEvent(new ActivityEvent("Completed", DateTimeOffset.Now));

                }
            }
            catch (Exception ex)
            {
                activity.AddException(ex, timestamp: DateTimeOffset.Now);
                throw;
            }
            finally
            {
                ZiganaDiagnosticSource.Instance.StopActivity(activity, null);
            }
        }
    }
}
