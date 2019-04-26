using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace FewBox.Core.Persistence.Orm
{
    public abstract class BaseRepository<TEntity, TID> : IBaseRepository<TEntity, TID> where TEntity: Entity<TID>
    {
        protected string TableName { get; set; }
        protected IUnitOfWork UnitOfWork { get; set; }
        protected ICurrentUser<TID> CurrentUser { get; set; }

        protected BaseRepository(string tableName, IOrmSession ormSession, ICurrentUser<TID> currentUser)
        {
            this.TableName = tableName;
            this.UnitOfWork = ormSession.UnitOfWork;
            this.CurrentUser = currentUser;
        }

        public int BatchSave(IEnumerable<TEntity> entities)
        {
            this.BatchInitSaveDefaultProperty(entities);
            return this.UnitOfWork.Connection.Execute(this.GetSaveSql(), entities);
        }

        public Task<int> BatchSaveAsync(IEnumerable<TEntity> entities)
        {
            this.BatchInitSaveDefaultProperty(entities);
            return this.UnitOfWork.Connection.ExecuteAsync(this.GetSaveSql(), entities);
        }

        public int Count()
        {
            return this.UnitOfWork.Connection.ExecuteScalar<int>(String.Format(@"select count(1) from {0}", this.TableName));
        }

        public Task<int> CountAsync()
        {
            return this.UnitOfWork.Connection.ExecuteScalarAsync<int>(String.Format(@"select count(1) from {0}", this.TableName));
        }

        public int Recycle(TID id)
        {
            this.UnitOfWork.Connection.Execute(String.Format(@"insert into {0}_recycle select * from {1} where Id=@id", this.TableName.Trim('`'), this.TableName), new { id });
            this.UnitOfWork.Connection.Execute(String.Format(@"update {0}_recycle set ModifiedBy=@ModifiedBy,ModifiedTime=@ModifiedTime where Id=@id", this.TableName.Trim('`')),
                new { id, ModifiedTime = DateTime.UtcNow, ModifiedBy = this.CurrentUser.GetId() });
            return this.UnitOfWork.Connection.Execute(String.Format(@"delete from {0} where Id=@id", this.TableName), new { id });

        }

        public Task<int> RecycleAsync(TID id)
        {
            this.UnitOfWork.Connection.ExecuteAsync(String.Format(@"insert into {0}_recycle select * from {1} where Id=@id", this.TableName.Trim('`'), this.TableName), new { id });
            this.UnitOfWork.Connection.ExecuteAsync(String.Format(@"update {0}_recycle set ModifiedBy=@ModifiedBy,ModifiedTime=@ModifiedTime where Id=@id", this.TableName.Trim('`')),
                new { id, ModifiedTime = DateTime.UtcNow, ModifiedBy = this.CurrentUser.GetId() });
            return this.UnitOfWork.Connection.ExecuteAsync(String.Format(@"delete from {0} where Id=@id", this.TableName), new { id });
        }

        public int Delete(TID id)
        {
            return this.UnitOfWork.Connection.Execute(String.Format(@"delete from {0} where Id=@id", this.TableName), new { id });

        }

        public Task<int> DeleteAsync(TID id)
        {
            return this.UnitOfWork.Connection.ExecuteAsync(String.Format(@"delete from {0} where Id=@id", this.TableName), new { id });
        }

        public IEnumerable<TEntity> FindAll()
        {
            return this.UnitOfWork.Connection.Query<TEntity>(String.Format(@"select * from {0}", this.TableName));
        }

        public Task<IEnumerable<TEntity>> FindAllAsync()
        {
            return this.UnitOfWork.Connection.QueryAsync<TEntity>(String.Format(@"select * from {0}", this.TableName));
        }

        public IEnumerable<TEntity> FindAll(int pageIndex, int pageRange)
        {
            int from = (pageIndex - 1) * pageRange;
            return this.UnitOfWork.Connection.Query<TEntity>(String.Format(@"select * from {0} limit @From,@PageRange", this.TableName), new { From = from, PageRange = pageRange });
        }

        public Task<IEnumerable<TEntity>> FindAllAsync(int pageIndex, int pageRange)
        {
            int from = (pageIndex - 1) * pageRange;
            return this.UnitOfWork.Connection.QueryAsync<TEntity>(String.Format(@"select * from {0} limit @From,@PageRange", this.TableName), new { From = from, PageRange = pageRange });
        }

        public TEntity FindOne(TID id)
        {
            return this.UnitOfWork.Connection.QuerySingleOrDefault<TEntity>(String.Format(@"select * from {0} where Id=@id", this.TableName), new { id });
        }

        public Task<TEntity> FindOneAsync(TID id)
        {
            return this.UnitOfWork.Connection.QuerySingleOrDefaultAsync<TEntity>(String.Format(@"select * from {0} where Id=@id", this.TableName), new { id });
        }

        public bool IsExist(TID id)
        {
            return this.UnitOfWork.Connection.ExecuteScalar<int>(String.Format("select count(1) from {0} where Id=@id", this.TableName), new { id }) > 0;
        }

        public Task<bool> IsExistAsync(TID id)
        {
            Task<int> task = this.UnitOfWork.Connection.ExecuteScalarAsync<int>(String.Format("select count(1) from {0} where Id=@id", TableName), new { id });
            return task.ContinueWith<bool>((t) => { return t.Result > 1; });

        }

        public TID Save(TEntity entity)
        {
            this.InitSaveDefaultProperty(entity);
            this.UnitOfWork.Connection.Execute(this.GetSaveSql(), entity);
            return entity.Id;
        }

        public Task<int> SaveAsync(TEntity entity)
        {
            this.InitSaveDefaultProperty(entity);
            if (entity.Id == null)
            {
                entity.Id = default(TID);
            }
            return this.UnitOfWork.Connection.ExecuteAsync(this.GetSaveSql(), entity);
        }

        public int Update(TEntity entity)
        {
            this.InitUpdateDefaultProperty(entity);
            return this.UnitOfWork.Connection.Execute(this.GetUpdateSql(), entity);

        }

        public Task<int> UpdateAsync(TEntity entity)
        {
            this.InitUpdateDefaultProperty(entity);
            return this.UnitOfWork.Connection.ExecuteAsync(this.GetUpdateSql(), entity);

        }

        protected void InitSaveDefaultProperty(TEntity entity)
        {
            entity.Id = this.GenerateNewId();
            entity.ModifiedTime = entity.CreatedTime = DateTime.UtcNow;
            entity.CreatedBy = this.CurrentUser.GetId();
            entity.ModifiedBy = this.CurrentUser.GetId();
        }

        private TID GenerateNewId()
        {
            if(typeof(TID).Name=="Guid")
            {
                return (TID)Convert.ChangeType(Guid.NewGuid(), typeof(TID));
            }
            else
            {
                return (TID)Convert.ChangeType(Guid.NewGuid().ToString(), typeof(TID));   
            }
        }

        protected void InitUpdateDefaultProperty(TEntity entity)
        {
            entity.ModifiedTime = DateTime.UtcNow;
            entity.ModifiedBy = this.CurrentUser.GetId();
        }

        protected void BatchInitSaveDefaultProperty(IEnumerable<TEntity> entitys)
        {
            foreach (var entity in entitys)
            {
                entity.ModifiedTime = entity.CreatedTime = DateTime.UtcNow;
                entity.ModifiedBy = entity.CreatedBy = this.CurrentUser.GetId();
            }
        }

        protected void BatchInitUpdateDefaultProperty(IEnumerable<TEntity> entitys)
        {
            foreach (var entity in entitys)
            {
                entity.ModifiedTime = entity.CreatedTime = DateTime.UtcNow;
                entity.ModifiedBy = entity.CreatedBy = this.CurrentUser.GetId();
            }
        }

        public Task<int> ReplaceAsync(TEntity entity)
        {
            this.InitSaveDefaultProperty(entity);
            return this.UnitOfWork.Connection.ExecuteAsync(this.GetReplaceSql(), entity);
        }

        public Task<int> BatchReplaceAsync(IEnumerable<TEntity> entities)
        {
            this.BatchInitSaveDefaultProperty(entities);
            return this.UnitOfWork.Connection.ExecuteAsync(this.GetReplaceSql(), entities);
        }

        public Task<int> BatchUpdateByUniqueKeyAsync(IEnumerable<TEntity> entities)
        {
            this.BatchInitUpdateDefaultProperty(entities);
            return this.UnitOfWork.Connection.ExecuteAsync(this.GetUpdateSqlWithUniqueKey(), entities);
        }

        public int Replace(TEntity entity)
        {
            this.InitSaveDefaultProperty(entity);
            return this.UnitOfWork.Connection.Execute(this.GetReplaceSql(), entity);
        }

        public int BatchReplace(IEnumerable<TEntity> entities)
        {
            this.BatchInitSaveDefaultProperty(entities);
            return this.UnitOfWork.Connection.Execute(this.GetReplaceSql(), entities);
        }

        public int BatchUpdateByUniqueKey(IEnumerable<TEntity> entities)
        {
            this.BatchInitUpdateDefaultProperty(entities);
            return this.UnitOfWork.Connection.Execute(this.GetUpdateSqlWithUniqueKey(), entities);
        }

        private string GetSaveSql()
        {
            var newSegments = from column in this.GetSaveSegmentSql().Split(',')
                              select String.Format(@"@{0}", column.Trim().Trim('`'));
            return String.Format(@"insert into {0} ({1}, Id, CreatedTime, ModifiedTime, CreatedBy, ModifiedBy) values({2}, @Id, @CreatedTime, @ModifiedTime, @CreatedBy, @ModifiedBy)",
                this.TableName, this.GetSaveSegmentSql(), String.Join(",", newSegments));
        }

        private string GetReplaceSql()
        {
            var newSegments = from column in this.GetSaveSegmentSql().Split(',')
                              select String.Format(@"@{0}", column.Trim().Trim('`'));
            return String.Format(@"replace into {0} ({1}, Id, CreatedTime, ModifiedTime, CreatedBy, ModifiedBy) values({2}, @Id, @CreatedTime, @ModifiedTime, @CreatedBy, @ModifiedBy)",
                this.TableName, this.GetSaveSegmentSql(), String.Join(",", newSegments));
        }

        private string GetUpdateSql()
        {
            var newSegments = from column in this.GetUpdateSegmentSql().Split(',')
                              select String.Format(@"{0}=@{1}", column.Trim(), column.Trim().Trim('`'));
            return String.Format(@"update {0} set {1}, ModifiedTime=@ModifiedTime, ModifiedBy=@ModifiedBy where Id = @Id", this.TableName, String.Join(",", newSegments));
        }

        private string GetUpdateSqlWithUniqueKey()
        {
            var newSegments = from column in this.GetUpdateSegmentSql().Split(',')
                              select String.Format(@"{0}=@{0}", column.Trim().Trim('`'));
            return String.Format(@"update {0} set {1}, ModifiedTime=@ModifiedTime, ModifiedBy=@ModifiedBy {2}", this.TableName, String.Join(",", newSegments), this.GetUpdateWithUniqueKeyWhereSegmentSql());
        }

        protected abstract string GetSaveSegmentSql();

        protected abstract string GetUpdateSegmentSql();

        protected abstract string GetUpdateWithUniqueKeyWhereSegmentSql();
    }
}
