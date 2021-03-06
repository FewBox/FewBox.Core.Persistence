﻿using System.Data;
using System.Data.Common;

namespace FewBox.Core.Persistence.Orm
{
    public abstract class OrmSession : IOrmSession
    {
        public IUnitOfWork UnitOfWork { get; set; }
        private IDbConnection Connection { get; set; }

        protected OrmSession(IOrmConfiguration ormConfiguration)
        {
            this.Connection = this.GetDbConnection(ormConfiguration.GetConnectionString());
            this.UnitOfWork = new UnitOfWork(this.Connection);
        }

        protected abstract DbConnection GetDbConnection(string connectionString);
    }
}
