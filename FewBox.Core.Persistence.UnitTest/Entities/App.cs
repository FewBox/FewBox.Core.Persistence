using FewBox.Core.Persistence.Orm;

namespace FewBox.Core.Persistence.UnitTest
{
    public class App : Entity<string>
    {
        public string Name { get; set; }
        public string Key { get; set; }
    }
}