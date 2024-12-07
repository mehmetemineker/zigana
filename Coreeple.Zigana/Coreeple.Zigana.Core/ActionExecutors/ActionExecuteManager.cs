using Coreeple.Zigana.Core.Abstractions;
using Coreeple.Zigana.Core.Types;
using Coreeple.Zigana.Core.Types.Actions;
using Microsoft.Extensions.DependencyInjection;

namespace Coreeple.Zigana.Core.ActionExecutors;
public class ActionExecuteManager : IActionExecuteManager
{
    private readonly Dictionary<Type, Func<Types.Action, CancellationToken, Task>> _executors;

    public ActionExecuteManager(IServiceProvider serviceProvider)
    {
        _executors = new Dictionary<Type, Func<Types.Action, CancellationToken, Task>>
        {
            {
                typeof(HttpRequestAction), async (action, token) => {
                    var executor = serviceProvider.GetRequiredService<IActionExecutor<HttpRequestAction>>();
                    await executor.ExecuteAsync((HttpRequestAction)action, token);
                }
            },
            {
                typeof(ParallelAction), async (action, token) => {
                    var executor = serviceProvider.GetRequiredService<IActionExecutor<ParallelAction>>();
                    await executor.ExecuteAsync((ParallelAction)action, token);
                }
            }
        };
    }

    public async Task StartAsync(Endpoint endpoint, CancellationToken cancellationToken = default)
    {
        if (endpoint.Actions == null)
        {
            return;
        }

        foreach (var (actionKey, action) in endpoint.Actions)
        {
            if (action == null)
            {
                continue;
            }

            if (_executors.TryGetValue(action.GetType(), out var executor))
            {
                await executor(action, cancellationToken);
            }
        }
    }
}
