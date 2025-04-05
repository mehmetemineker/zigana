using Zigana.Data.Abstractions;

namespace Zigana.Data.Entities;
public class EndpointRequestTransaction : IEntity
{
    public required Guid Id { get; set; }
    public required Guid EndpointId { get; set; }
    public required string Name { get; set; }
    public required string Status { get; set; }
    public string? Message { get; set; }
    public required DateTime Date { get; set; }
}
