using System.Data;

namespace GoldenRaspberryAwards.Infrastructure.Data
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}
