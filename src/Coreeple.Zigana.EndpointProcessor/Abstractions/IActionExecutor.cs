using System.Text.Json.Nodes;
using Action = Coreeple.Zigana.Core.Types.Action;

namespace Coreeple.Zigana.EndpointProcessor.Abstractions;
public interface IActionExecutor<T> where T : Action
{
    Task<JsonNode?> ExecuteAsync(T action, CancellationToken cancellationToken);
}
