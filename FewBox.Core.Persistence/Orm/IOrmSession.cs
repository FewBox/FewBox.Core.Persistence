using System;

namespace FewBox.Core.Persistence.Orm
{
    public interface IOrmSession
    {
        IUnitOfWork UnitOfWork { get; set; }
        void Close();
    }
}
