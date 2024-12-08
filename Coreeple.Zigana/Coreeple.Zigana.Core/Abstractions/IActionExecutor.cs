namespace Coreeple.Zigana.Core.Abstractions;
public interface IActionExecutor<T> where T : Types.Action
{
    Task ExecuteAsync(T action, CancellationToken cancellationToken);
}
