namespace Coreeple.Zigana.Services.Dtos;
public class EndpointTransactionCreateDto
{
    public Guid EndpointId { get; set; }
    public Guid RequestId { get; set; }
    public required string Name { get; set; }
    public required string Status { get; set; }
}
