using System;

namespace FewBox.Core.Persistence.Orm
{
    public interface ICurrentUser<TID>
    {
        TID GetId();
    }
}
