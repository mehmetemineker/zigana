using Coreeple.Zigana.Core.Types;
using System.Text.Json.Nodes;

namespace Coreeple.Zigana.Core.Abstractions;

public interface IActionExecuteManager
{
    Task RunAsync(Endpoint endpoint, JsonObject context, CancellationToken cancellationToken = default);
}