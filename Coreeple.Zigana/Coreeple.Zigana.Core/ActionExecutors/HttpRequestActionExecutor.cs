using Coreeple.Zigana.Core.Abstractions;
using Coreeple.Zigana.Core.Types.Actions;
using System.Text.Json.Nodes;

namespace Coreeple.Zigana.Core.ActionExecutors;
public class HttpRequestActionExecutor : IActionExecutor<HttpRequestAction>
{
    public async Task<JsonNode?> ExecuteAsync(HttpRequestAction action, CancellationToken cancellationToken)
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
