using Coreeple.Zigana.Core.Types;
using System.Text.Json.Nodes;

namespace Coreeple.Zigana.Core.ActionExecutors;
public class ParallelActionExecutor : IActionExecutor<ParallelAction>
{
    public async Task<JsonNode?> ExecuteAsync(ParallelAction action, CancellationToken cancellationToken)
    {
        await Task.Delay(2000, cancellationToken);

        return new JsonObject
        {
            ["status"] = "Ok",
            ["content"] = new JsonObject
            {
                ["message"] = "Hello, World!"
            }
        };
    }
}
