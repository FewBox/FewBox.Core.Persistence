using MySql.Data.MySqlClient;
using System.Data.Common;

namespace FewBox.Core.Persistence.Orm
{
    public class MySqlSession : OrmSession
    {
        public MySqlSession(IOrmConfiguration ormConfiguration) 
        : base(ormConfiguration)
        {
        }

        protected override DbConnection GetDbConnection(string connectionString)
        {
            return new MySqlConnection(connectionString);
        }
    }
}
