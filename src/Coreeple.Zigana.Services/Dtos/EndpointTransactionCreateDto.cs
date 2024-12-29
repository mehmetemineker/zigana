namespace Coreeple.Zigana.Services.Dtos;
public class EndpointRequestTransactionCreateDto
{
    public Guid Id { get; set; }
    public Guid EndpointId { get; set; }
    public required string Name { get; set; }
    public required string Status { get; set; }
    public string? Message { get; set; }
    public DateTime Date { get; set; }
}
