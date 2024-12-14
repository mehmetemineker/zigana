namespace Coreeple.Zigana.Core.Abstractions;

public interface IActionExecuteManager
{
    Task RunAsync(Dictionary<string, Types.Action> actions, CancellationToken cancellationToken = default);
}