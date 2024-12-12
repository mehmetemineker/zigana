namespace Coreeple.Zigana.Core.Data.Entities;
public class EndpointTransaction
{
    public Guid Id { get; set; }
    public Guid EndpointId { get; set; }
    public Guid RequestId { get; set; }
    public string? Name { get; set; }
    public DateTime Date { get; set; }
    public string? Status { get; set; }
}
