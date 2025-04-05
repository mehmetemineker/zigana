namespace Zigana.Services.Dtos;
public class CreateEndpointDto
{
    public Guid ApiId { get; set; }
    public required string Path { get; set; }
    public required string Method { get; set; }
    public string? Actions { get; set; }
    public string? Response { get; set; }
}
