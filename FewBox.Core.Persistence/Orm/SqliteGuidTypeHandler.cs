using System;
using System.Data;

namespace FewBox.Core.Persistence.Orm
{
    public class SqliteGuidTypeHandler : Dapper.SqlMapper.TypeHandler<Guid>
    {
        public override Guid Parse(object value)
        {
            Guid guid = Guid.Empty;
            if(value is byte[])
            {
                guid = new Guid((byte[])value);
            }
            return guid;
        }

        public override void SetValue(IDbDataParameter parameter, Guid value)
        {
            parameter.Value = value.ToString();
        }
    }
}
