using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FewBox.Core.Persistence.Orm
{
    public interface IBaseRepository<TEntity, TID>
    {
        #region Async

        Task<int> CountAsync();
        Task<IEnumerable<TEntity>> FindAllAsync();
        Task<IEnumerable<TEntity>> FindAllAsync(int pageIndex, int pageRange);
        Task<TEntity> FindOneAsync(TID id);
        Task<int> SaveAsync(TEntity entity);
        Task<int> ReplaceAsync(TEntity entity);
        Task<int> UpdateAsync(TEntity entity);
        Task<int> DeleteAsync(TID id);
        Task<int> RecycleAsync(TID id);
        Task<bool> IsExistAsync(TID id);
        Task<int> BatchSaveAsync(IEnumerable<TEntity> entities);
        Task<int> BatchReplaceAsync(IEnumerable<TEntity> entities);
        Task<int> BatchUpdateByUniqueKeyAsync(IEnumerable<TEntity> entities);
        Task<int> ClearAsync();

        #endregion

        #region Sync

        int Count();
        IEnumerable<TEntity> FindAll();
        IEnumerable<TEntity> FindAll(int pageIndex, int pageRange);
        TEntity FindOne(TID id);
        TID Save(TEntity entity);
        int Replace(TEntity entity);
        int Update(TEntity entity);
        int Delete(TID id);
        int Recycle(TID id);
        bool IsExist(TID id);
        int BatchSave(IEnumerable<TEntity> entities);
        int BatchReplace(IEnumerable<TEntity> entities);
        int BatchUpdateByUniqueKey(IEnumerable<TEntity> entities);
        int Clear();

        #endregion
    }
}
