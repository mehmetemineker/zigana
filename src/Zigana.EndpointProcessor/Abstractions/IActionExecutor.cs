using System.Text.Json.Nodes;
using Action = Zigana.Core.Types.Action;

namespace Zigana.EndpointProcessor.Abstractions;
public interface IActionExecutor<T> where T : Action
{
    Task<JsonNode?> ExecuteAsync(T action, CancellationToken cancellationToken);
}
