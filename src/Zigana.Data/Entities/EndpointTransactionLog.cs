using Zigana.Data.Abstractions;

namespace Zigana.Data.Entities;
public class EndpointTransactionLog : IEntity
{
    public Guid Id { get; set; }
    public Guid TransactionId { get; set; }
    public string? Level { get; set; }
    public string? Log { get; set; }
    public DateTime Date { get; set; }
}
