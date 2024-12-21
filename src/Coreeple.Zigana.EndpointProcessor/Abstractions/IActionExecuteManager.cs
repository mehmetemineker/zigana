using Action = Coreeple.Zigana.Core.Types.Action;

namespace Coreeple.Zigana.EndpointProcessor.Abstractions;

public interface IActionExecuteManager
{
    Task RunAsync(Dictionary<string, Action> actions, CancellationToken cancellationToken = default);
}