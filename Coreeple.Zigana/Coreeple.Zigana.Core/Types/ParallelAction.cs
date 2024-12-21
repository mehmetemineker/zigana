namespace Coreeple.Zigana.Core.Types;

public class ParallelAction : Action
{
    public required Dictionary<string, Action> Actions { get; set; }
}
