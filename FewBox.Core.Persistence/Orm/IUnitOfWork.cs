using System;
using System.Data;

namespace FewBox.Core.Persistence.Orm
{
    public interface IUnitOfWork : IDisposable
    {
        IDbConnection Connection { get; }
        IDbTransaction Transaction { get; }
        void Reset();
        void Commit();
        void Rollback();
    }
}
