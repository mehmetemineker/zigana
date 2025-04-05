namespace Zigana.Services.Dtos;
public class CreateApiDto
{
    public required string Path { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Defs { get; set; }
}
