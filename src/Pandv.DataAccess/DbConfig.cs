using System.Collections.Generic;

namespace Pandv.DataAccess
{
    public class DbConfig
    {
        public List<DataConnection> ConnectionStrings { get; set; }

        public List<DbSql> SqlConfigs { get; set; }

        internal Dictionary<string, DbSql> Sqls { get; set; }
    }
}