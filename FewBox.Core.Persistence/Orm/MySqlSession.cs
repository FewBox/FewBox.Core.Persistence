using FewBox.Core.Persistence.Cache;
using MySql.Data.MySqlClient;
using System.Data.Common;

namespace FewBox.Core.Persistence.Orm
{
    public class MySqlSession : OrmSession
    {
        public MySqlSession(IOrmConfiguration ormConfiguration, ITokenService tokenService) 
        : base(ormConfiguration, tokenService)
        {
        }

        protected override DbConnection GetDbConnection(string connectionString)
        {
            return new MySqlConnection(connectionString);
        }
    }
}
