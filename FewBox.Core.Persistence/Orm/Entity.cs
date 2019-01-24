using System;

namespace FewBox.Core.Persistence.Orm
{
    public class Entity<T>
    {
        public T Id { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime ModifiedTime { get; set; }
        public T CreatedBy { get; set; }
        public T ModifiedBy { get; set; }
    }
}
