using System;

namespace FewBox.Core.Persistence.Orm
{
    public interface IRepository<TEntity> : IBaseRepository<TEntity, Guid>
    {
    }
}