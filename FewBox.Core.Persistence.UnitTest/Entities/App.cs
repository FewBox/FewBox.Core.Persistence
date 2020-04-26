using System;
using FewBox.Core.Persistence.Orm;

namespace FewBox.Core.Persistence.UnitTest
{
    public class App : Entity
    {
        public string Name { get; set; }
        public string Key { get; set; }
        public Guid Char36Id { get; set; }
    }
}