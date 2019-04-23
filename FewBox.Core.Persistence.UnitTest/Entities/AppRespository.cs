using FewBox.Core.Persistence.Orm;

namespace FewBox.Core.Persistence.UnitTest
{
    public class AppRespository : BaseRepository<App, string>, IAppRespository
    {
        public AppRespository(string tableName, IOrmSession ormSession, ICurrentUser<string> currentUser) 
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