using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace FewBox.Core.Persistence.Orm
{
    public abstract class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity: Entity
    {
        protected string TableName { get; set; }
        protected IUnitOfWork UnitOfWork { get; set; }

        protected BaseRepository(string tableName, IDapperSession dapperSession)
        {
            this.TableName = tableName;
            this.UnitOfWork = dapperSession.UnitOfWork;
        }

        public int BatchSave(IEnumerable<TEntity> entities)
        {
            this.BatchInitSaveDefaultProperty(entities, this.UnitOfWork.CurrentUser.Id);
            return this.UnitOfWork.Connection.Execute(this.GetSaveSql(), entities);
        }

        public Task<int> BatchSaveAsync(IEnumerable<TEntity> entities)
        {
            this.BatchInitSaveDefaultProperty(entities, this.UnitOfWork.CurrentUser.Id);
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

        public int Recycle(Guid id)
        {
            this.UnitOfWork.Connection.Execute(String.Format(@"insert into {0}_recycle select * from {1} where Id=@id", this.TableName.Trim('`'), this.TableName), new { id });
            this.UnitOfWork.Connection.Execute(String.Format(@"update {0}_recycle set ModifiedBy=@ModifiedBy,ModifiedTime=@ModifiedTime where Id=@id", this.TableName.Trim('`')),
                new { id, ModifiedTime = DateTime.UtcNow, ModifiedBy = this.UnitOfWork.CurrentUser.Id });
            return this.UnitOfWork.Connection.Execute(String.Format(@"delete from {0} where Id=@id", this.TableName), new { id });

        }

        public Task<int> RecycleAsync(Guid id)
        {
            this.UnitOfWork.Connection.ExecuteAsync(String.Format(@"insert into {0}_recycle select * from {1} where Id=@id", this.TableName.Trim('`'), this.TableName), new { id });
            this.UnitOfWork.Connection.ExecuteAsync(String.Format(@"update {0}_recycle set ModifiedBy=@ModifiedBy,ModifiedTime=@ModifiedTime where Id=@id", this.TableName.Trim('`')),
                new { id, ModifiedTime = DateTime.UtcNow, ModifiedBy = this.UnitOfWork.CurrentUser.Id });
            return this.UnitOfWork.Connection.ExecuteAsync(String.Format(@"delete from {0} where Id=@id", this.TableName), new { id });
        }

        public int Delete(Guid id)
        {
            return this.UnitOfWork.Connection.Execute(String.Format(@"delete from {0} where Id=@id", this.TableName), new { id });

        }

        public Task<int> DeleteAsync(Guid id)
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

        public TEntity FindOne(Guid id)
        {
            return this.UnitOfWork.Connection.QuerySingleOrDefault<TEntity>(String.Format(@"select * from {0} where Id=@id", this.TableName), new { id });
        }

        public Task<TEntity> FindOneAsync(Guid id)
        {
            return this.UnitOfWork.Connection.QuerySingleOrDefaultAsync<TEntity>(String.Format(@"select * from {0} where Id=@id", this.TableName), new { id });
        }

        public bool IsExist(Guid id)
        {
            return this.UnitOfWork.Connection.ExecuteScalar<int>(String.Format("select count(1) from {0} where Id=@id", this.TableName), new { id }) > 0;
        }

        public Task<bool> IsExistAsync(Guid id)
        {
            Task<int> task = this.UnitOfWork.Connection.ExecuteScalarAsync<int>(String.Format("select count(1) from {0} where Id=@id", TableName), new { id });
            return task.ContinueWith<bool>((t) => { return t.Result > 1; });

        }

        public Guid Save(TEntity entity)
        {
            this.InitCreatedAndModified(entity, this.UnitOfWork.CurrentUser.Id);
            if (entity.Id == Guid.Empty)
            {
                entity.Id = Guid.NewGuid();
            }
            this.UnitOfWork.Connection.Execute(this.GetSaveSql(), entity);
            return entity.Id;
        }

        public Task<int> SaveAsync(TEntity entity)
        {
            this.InitCreatedAndModified(entity, this.UnitOfWork.CurrentUser.Id);
            if (entity.Id == Guid.Empty)
            {
                entity.Id = Guid.NewGuid();
            }
            return this.UnitOfWork.Connection.ExecuteAsync(this.GetSaveSql(), entity);
        }

        public int Update(TEntity entity)
        {
            this.InitUpdateDefaultProperty(entity, this.UnitOfWork.CurrentUser.Id);
            return this.UnitOfWork.Connection.Execute(this.GetUpdateSql(), entity);

        }

        public Task<int> UpdateAsync(TEntity entity)
        {
            this.InitUpdateDefaultProperty(entity, this.UnitOfWork.CurrentUser.Id);
            return this.UnitOfWork.Connection.ExecuteAsync(this.GetUpdateSql(), entity);

        }

        protected void InitCreatedAndModified(Entity entity, string currentUserId)
        {
            entity.ModifiedTime = entity.CreatedTime = DateTime.UtcNow;
            if (String.IsNullOrEmpty(entity.CreatedBy))
            {
                entity.CreatedBy = currentUserId;
            }
            if (String.IsNullOrEmpty(entity.ModifiedBy))
            {
                entity.ModifiedBy = currentUserId;
            }
        }

        protected void BatchInitSaveDefaultProperty(IEnumerable<TEntity> entitys, string currentUserId)
        {
            foreach (var entity in entitys)
            {
                entity.ModifiedTime = entity.CreatedTime = DateTime.UtcNow;
                entity.ModifiedBy = entity.CreatedBy = currentUserId;
            }
        }

        protected void BatchInitUpdateDefaultProperty(IEnumerable<TEntity> entitys, string currentUserId)
        {
            foreach (var entity in entitys)
            {
                entity.ModifiedTime = entity.CreatedTime = DateTime.UtcNow;
                entity.ModifiedBy = entity.CreatedBy = currentUserId;
            }
        }

        public Task<int> ReplaceAsync(TEntity entity)
        {
            this.InitCreatedAndModified(entity, this.UnitOfWork.CurrentUser.Id);
            return this.UnitOfWork.Connection.ExecuteAsync(this.GetReplaceSql(), entity);
        }

        public Task<int> BatchReplaceAsync(IEnumerable<TEntity> entities)
        {
            this.BatchInitSaveDefaultProperty(entities, this.UnitOfWork.CurrentUser.Id);
            return this.UnitOfWork.Connection.ExecuteAsync(this.GetReplaceSql(), entities);
        }

        public Task<int> BatchUpdateByUniqueKeyAsync(IEnumerable<TEntity> entities)
        {
            this.BatchInitUpdateDefaultProperty(entities, this.UnitOfWork.CurrentUser.Id);
            return this.UnitOfWork.Connection.ExecuteAsync(this.GetUpdateSqlWithUniqueKey(), entities);
        }

        public int Replace(TEntity entity)
        {
            this.InitCreatedAndModified(entity, this.UnitOfWork.CurrentUser.Id);
            return this.UnitOfWork.Connection.Execute(this.GetReplaceSql(), entity);
        }

        public int BatchReplace(IEnumerable<TEntity> entities)
        {
            this.BatchInitSaveDefaultProperty(entities, this.UnitOfWork.CurrentUser.Id);
            return this.UnitOfWork.Connection.Execute(this.GetReplaceSql(), entities);
        }

        public int BatchUpdateByUniqueKey(IEnumerable<TEntity> entities)
        {
            this.BatchInitUpdateDefaultProperty(entities, this.UnitOfWork.CurrentUser.Id);
            return this.UnitOfWork.Connection.Execute(this.GetUpdateSqlWithUniqueKey(), entities);
        }

        protected void InitUpdateDefaultProperty(Entity entity, string currentUserId)
        {
            entity.ModifiedTime = DateTime.UtcNow;
            entity.ModifiedBy = currentUserId;
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
