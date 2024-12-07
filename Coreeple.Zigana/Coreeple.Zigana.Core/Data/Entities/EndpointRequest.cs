namespace Coreeple.Zigana.Core.Data.Entities;
public class EndpointRequest
{
    public Guid Id { get; set; }
    public Guid EndpointId { get; set; }
    public DateTime BeginDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Status { get; set; }
}
