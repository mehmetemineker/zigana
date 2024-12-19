namespace Coreeple.Zigana.Core.Data.Entities;
public class Endpoint
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ApiId { get; set; }
    public required string Path { get; set; }
    public required string Method { get; set; }
    public string? Actions { get; set; }
    public string? Response { get; set; }
    public string? Defs { get; set; }
}

