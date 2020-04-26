using System;

namespace FewBox.Core.Persistence.Orm
{
    public abstract class Repository<TEntity> : BaseRepository<TEntity, Guid>, IRepository<TEntity> where TEntity : BaseEntity<Guid>
    {
        protected Repository(string tableName, IOrmSession ormSession, ICurrentUser<Guid> currentUser) : base(tableName, ormSession, currentUser)
        {
        }

        protected override Guid GetID(Guid originalId)
        {
            Guid id;
            if (originalId == Guid.Empty)
            {
                id = Guid.NewGuid();
            }
            else
            {
                id = originalId;
            }
            return id;
        }
    }
}
