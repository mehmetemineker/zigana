using System.Text.Json.Nodes;

namespace Coreeple.Zigana.Core.Abstractions;

public interface IActionExecuteManager
{
    Task RunAsync(Dictionary<string, Types.Action> actions, JsonObject context, CancellationToken cancellationToken = default);
}