using System.Data;

namespace Persistence
{
    public interface IDatabaseContext
    {
        IDbConnection CreateConnection();    
    }
}
