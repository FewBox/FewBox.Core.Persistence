using System;

namespace FewBox.Core.Persistence.Orm
{
    public class Entity
    {
        public Guid Id { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime ModifiedTime { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
    }
}
