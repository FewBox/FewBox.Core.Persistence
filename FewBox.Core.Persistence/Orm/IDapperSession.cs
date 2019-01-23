using System;

namespace FewBox.Core.Persistence.Orm
{
    public interface IDapperSession
    {
        IUnitOfWork UnitOfWork { get; set; }
        Guid Id { get; set; }
        void Close();
    }
}
