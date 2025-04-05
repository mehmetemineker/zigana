using System.Data;

namespace Zigana.Data.Abstractions;
public interface IDbContext
{
    IDbConnection CreateConnection();
    void Migration();
}