namespace Coreeple.Zigana.Core.Data.Entities;
public class EndpointTransactionLog
{
    public Guid Id { get; set; }
    public Guid TransactionId { get; set; }
    public string? Level { get; set; }
    public string? Log { get; set; }
    public DateTime Date { get; set; }
}
