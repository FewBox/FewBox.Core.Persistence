using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FewBox.Core.Persistence.Orm
{
    public interface IBaseRepository<TEntity>
    {
        #region Async

        Task<int> CountAsync();
        Task<IEnumerable<TEntity>> FindAllAsync();
        Task<IEnumerable<TEntity>> FindAllAsync(int pageIndex, int pageRange);
        Task<TEntity> FindOneAsync(Guid id);
        Task<int> SaveAsync(TEntity entity);
        Task<int> ReplaceAsync(TEntity entity);
        Task<int> UpdateAsync(TEntity entity);
        Task<int> DeleteAsync(Guid id);
        Task<int> RecycleAsync(Guid id);
        Task<bool> IsExistAsync(Guid id);
        Task<int> BatchSaveAsync(IEnumerable<TEntity> entities);
        Task<int> BatchReplaceAsync(IEnumerable<TEntity> entities);
        Task<int> BatchUpdateByUniqueKeyAsync(IEnumerable<TEntity> entities);

        #endregion

        #region Sync

        int Count();
        IEnumerable<TEntity> FindAll();
        IEnumerable<TEntity> FindAll(int pageIndex, int pageRange);
        TEntity FindOne(Guid id);
        Guid Save(TEntity entity);
        int Replace(TEntity entity);
        int Update(TEntity entity);
        int Delete(Guid id);
        int Recycle(Guid id);
        bool IsExist(Guid id);
        int BatchSave(IEnumerable<TEntity> entities);
        int BatchReplace(IEnumerable<TEntity> entities);
        int BatchUpdateByUniqueKey(IEnumerable<TEntity> entities);

        #endregion
    }
}
