using System;
using FewBox.Core.Persistence.Orm;

namespace FewBox.Core.Persistence.UnitTest
{
    public class AppRespository : BaseRepository<App, Guid>, IAppRespository
    {
        public AppRespository(string tableName, IOrmSession ormSession, ICurrentUser<Guid> currentUser) 
        : base(tableName, ormSession, currentUser)
        {
        }

        protected override string GetSaveSegmentSql()
        {
            return "Name,`Key`";
        }

        protected override string GetUpdateSegmentSql()
        {
            return "Name,`Key`";
        }

        protected override string GetUpdateWithUniqueKeyWhereSegmentSql()
        {
            throw new System.NotImplementedException();
        }
    }
}