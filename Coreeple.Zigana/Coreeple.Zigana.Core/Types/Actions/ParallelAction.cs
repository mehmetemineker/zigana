namespace Coreeple.Zigana.Core.Types.Actions;

public class ParallelAction : Action
{
    public required string Continue { get; set; }
    public required Dictionary<string, Action> Actions { get; set; }
}
