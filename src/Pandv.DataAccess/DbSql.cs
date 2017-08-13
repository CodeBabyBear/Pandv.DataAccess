using Dapper;
using System.Data;
using System.Threading;
using System.Xml.Serialization;

namespace Pandv.DataAccess
{
    public class DbSql
    {
        [XmlAttribute]
        public string CommandName { get; set; }

        public string ConnectionString { get; set; }

        public string Text { get; set; }

        [XmlAttribute]
        public CommandType Type { get; set; }

        [XmlAttribute]
        public int Timeout { get; set; }

        [XmlAttribute]
        public string ConnectionName { get; set; }

        public DbSql Clone()
        {
            return (DbSql)this.MemberwiseClone();
        }

        public CommandDefinition ToCommandDefinition(object parameters, IDbTransaction transaction = null, CommandFlags flags = CommandFlags.Buffered, CancellationToken cancellationToken = default(CancellationToken))
        {
            int? timeout = null;
            if (Timeout > 0)
                timeout = Timeout;
            return new CommandDefinition(Text, parameters, transaction, timeout, Type, flags, cancellationToken);
        }
    }
}