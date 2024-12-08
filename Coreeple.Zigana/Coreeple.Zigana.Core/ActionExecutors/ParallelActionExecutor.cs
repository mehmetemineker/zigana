using Coreeple.Zigana.Core.Abstractions;
using Coreeple.Zigana.Core.Types.Actions;

namespace Coreeple.Zigana.Core.ActionExecutors;
public class ParallelActionExecutor : IActionExecutor<ParallelAction>
{
    public async Task ExecuteAsync(ParallelAction action, CancellationToken cancellationToken)
    {
        await Task.Delay(2000, cancellationToken);
    }
}
