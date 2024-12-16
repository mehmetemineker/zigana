namespace Coreeple.Zigana.Core.Data.Entities;
public class EndpointTransaction
{
    public required Guid Id { get; set; }
    public required Guid EndpointId { get; set; }
    public required Guid RequestId { get; set; }
    public required string Name { get; set; }
    public required string Status { get; set; }
    public required DateTime Date { get; set; }
}
