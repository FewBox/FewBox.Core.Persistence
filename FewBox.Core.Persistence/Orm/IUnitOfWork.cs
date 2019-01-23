using System;
using System.Data;

namespace FewBox.Core.Persistence.Orm
{
    public interface IUnitOfWork : IDisposable
    {
        IDbConnection Connection { get; }
        IDbTransaction Transaction { get; }
        CurrentUser CurrentUser { get; }
        void Reset();
        void Commit();
        void Rollback();
    }
}
