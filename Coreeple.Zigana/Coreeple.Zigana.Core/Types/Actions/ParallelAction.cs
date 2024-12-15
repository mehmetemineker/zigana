namespace Coreeple.Zigana.Core.Types.Actions;

public class ParallelAction : Action
{
    public required Dictionary<string, Action> Actions { get; set; }
}
