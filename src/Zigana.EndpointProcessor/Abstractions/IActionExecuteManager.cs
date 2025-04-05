using Action = Zigana.Core.Types.Action;

namespace Zigana.EndpointProcessor.Abstractions;

public interface IActionExecuteManager
{
    Task RunAsync(Dictionary<string, Action> actions, CancellationToken cancellationToken = default);
}