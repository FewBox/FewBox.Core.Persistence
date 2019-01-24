using FewBox.Core.Persistence.Orm;

namespace FewBox.Core.Persistence.UnitTest
{
    public interface IAppRespository : IBaseRepository<App, string>
    {
    }
}