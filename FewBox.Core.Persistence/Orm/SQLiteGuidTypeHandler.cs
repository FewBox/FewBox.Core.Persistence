using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace FewBox.Core.Persistence.Orm
{
    public class SQLiteGuidTypeHandler : Dapper.SqlMapper.TypeHandler<Guid>
    {
        public override Guid Parse(object value)
        {
            Guid guid = Guid.Empty;
            if(value is byte[])
            {
                guid = new Guid((byte[])value);
            }
            else if(value is string)
            {
                guid = new Guid(value.ToString());
            }
            return guid;
        }

        public override void SetValue(IDbDataParameter parameter, Guid value)
        {
            parameter.Value = value.ToByteArray();
        }
    }
}
