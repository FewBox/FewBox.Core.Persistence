using FewBox.Core.Persistence.Cache;
using System.Data.Common;
using System.Data.SQLite;

namespace FewBox.Core.Persistence.Orm
{
    public class SQLiteSession : OrmSession
    {
        public SQLiteSession(IOrmConfiguration ormConfiguration, ITokenService tokenService) 
        : base(ormConfiguration, tokenService)
        {
        }

        protected override DbConnection GetDbConnection(string connectionString)
        {
            return new SQLiteConnection(connectionString);
        }
    }
}
