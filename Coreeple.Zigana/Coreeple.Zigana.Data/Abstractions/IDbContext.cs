using System.Data;

namespace Coreeple.Zigana.Data.Abstractions;
public interface IDbContext
{
    public IDbConnection CreateConnection();
}