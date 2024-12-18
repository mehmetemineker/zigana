namespace Coreeple.Zigana.Core.Types.Actions;
public class HtmlParserAction : Action
{
    public required string Source { get; set; }
    public required Dictionary<string, string> Selectors { get; set; }
}
