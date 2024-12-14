using Coreeple.Zigana.Core.Types;

namespace Coreeple.Zigana.Core.Abstractions;

public interface IActionExecuteManager
{
    Task RunAsync(Endpoint endpoint, CancellationToken cancellationToken = default);
}