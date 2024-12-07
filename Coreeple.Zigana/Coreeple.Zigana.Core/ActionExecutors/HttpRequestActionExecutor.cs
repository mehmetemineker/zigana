using System.Text.Json.Nodes;

namespace Coreeple.Zigana.Core.ActionExecutors;
public class HttpRequestActionExecutor
{
    public async Task<JsonNode?> ExecuteAsync(CancellationToken cancellationToken)
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
