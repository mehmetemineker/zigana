using Coreeple.Zigana.Core.Types;

namespace Coreeple.Zigana.Core.ActionExecutors;
public interface IActionExecuteManager
{
    Task StartAsync(Endpoint endpoint, CancellationToken cancellationToken = default);
}