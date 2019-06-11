using System.Data;

namespace FewBox.Core.Persistence.Orm
{
    public class UnitOfWork : IUnitOfWork
    {
        public IDbConnection Connection { get; private set; }
        public IDbTransaction Transaction { get; private set; }

        public UnitOfWork(IDbConnection connection)
        {
            this.Connection = connection;
        }

        public void Start()
        {
            this.Connection.Open();
            this.Transaction = this.Connection.BeginTransaction();
        }

        public void Reset()
        {
            this.Transaction = this.Connection.BeginTransaction();
        }

        public void Commit()
        {
            this.Transaction.Commit();
        }

        public void Rollback()
        {
            this.Transaction.Rollback();
        }

        public void Dispose()
        {
            this.Connection.Close();
            this.Connection.Dispose();
            this.Transaction.Dispose();
        }        
    }
}
