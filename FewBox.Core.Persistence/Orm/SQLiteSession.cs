using System.Data.Common;
using Microsoft.Data.Sqlite;

namespace FewBox.Core.Persistence.Orm
{
    public class SQLiteSession : OrmSession
    {
        public SQLiteSession(IOrmConfiguration ormConfiguration) 
        : base(ormConfiguration)
        {
        }

        protected override DbConnection GetDbConnection(string connectionString)
        {
            return new SqliteConnection(connectionString);
        }
    }
}
