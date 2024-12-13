using System.Text.Json.Nodes;

namespace Coreeple.Zigana.Core.Types.Actions;
public class HtmlParserAction : Action
{
    public required string Source { get; set; }
    public required JsonNode Selectors { get; set; }
}
