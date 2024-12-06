namespace Coreeple.Zigana.Core.Data.Entities;
public class EndpointRequestLog
{
    public Guid Id { get; set; }
    public Guid EndpointRequestId { get; set; }
    public string? Log { get; set; }
}
