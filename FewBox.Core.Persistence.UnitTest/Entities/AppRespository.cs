using FewBox.Core.Persistence.Orm;
using Microsoft.AspNetCore.Http;

namespace FewBox.Core.Persistence.UnitTest
{
    public class AppRespository : BaseRepository<App, string>, IAppRespository
    {
        public AppRespository(string tableName, IOrmSession dapperSession, ICurrentUser<string> currentUser) 
        : base(tableName, dapperSession, currentUser)
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