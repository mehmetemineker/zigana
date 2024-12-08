using System.Text.Json.Nodes;

namespace Coreeple.Zigana.Core.Abstractions;
public interface IActionExecutor<T> where T : Types.Action
{
    Task<JsonNode?> ExecuteAsync(T action, CancellationToken cancellationToken);
}
