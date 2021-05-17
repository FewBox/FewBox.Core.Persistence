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
        Task<IEnumerable<TEntity>> FindAllOrderByAsync(IEnumerable<string> fields, OrderType orderType);
        Task<IEnumerable<TEntity>> FindAllOrderByAsync(IDictionary<string, OrderType> fieldOrders);
        Task<IEnumerable<TEntity>> FindAllAsync(int pageIndex, int pageRange);
        Task<IEnumerable<TEntity>> FindAllOrderByAsync(int pageIndex, int pageRange, IEnumerable<string> fields, OrderType orderType);
        Task<IEnumerable<TEntity>> FindAllOrderByAsync(int pageIndex, int pageRange, IDictionary<string, OrderType> fieldOrders);
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
        Task<int> CountByCreatedByAsync(Guid createdBy);
        Task<int> CountByModifiedByAsync(Guid modifiedBy);
        Task<IEnumerable<TEntity>> FindAllByCreatedByAsync(Guid createdBy);
        Task<IEnumerable<TEntity>> FindAllByModifiedByAsync(Guid modifiedBy);
        Task<IEnumerable<TEntity>> FindAllByCreatedByAsync(Guid createdBy, int pageIndex, int pageRange);
        Task<IEnumerable<TEntity>> FindAllByModifiedByAsync(Guid modifiedBy, int pageIndex, int pageRange);

        #endregion

        #region Sync

        int Count();
        IEnumerable<TEntity> FindAll();
        IEnumerable<TEntity> FindAllOrderBy(IEnumerable<string> fields, OrderType orderType);
        IEnumerable<TEntity> FindAllOrderBy(IDictionary<string, OrderType> fieldOrders);
        IEnumerable<TEntity> FindAll(int pageIndex, int pageRange);
        IEnumerable<TEntity> FindAllOrderBy(int pageIndex, int pageRange, IEnumerable<string> fields, OrderType orderType);
        IEnumerable<TEntity> FindAllOrderBy(int pageIndex, int pageRange, IDictionary<string, OrderType> fieldOrders);
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
        int CountByCreatedBy(Guid createdBy);
        int CountByModifiedBy(Guid modifiedBy);
        IEnumerable<TEntity> FindAllByCreatedBy(Guid createdBy);
        IEnumerable<TEntity> FindAllByModifiedBy(Guid modifiedBy);
        IEnumerable<TEntity> FindAllByCreatedBy(Guid createdBy, int pageIndex, int pageRange);
        IEnumerable<TEntity> FindAllByModifiedBy(Guid modifiedBy, int pageIndex, int pageRange);
        #endregion
    }
}
