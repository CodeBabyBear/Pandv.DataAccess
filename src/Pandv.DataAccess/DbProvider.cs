using Dapper;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using VIC.ObjectConfig;
using VIC.ObjectConfig.Abstraction;

namespace Pandv.DataAccess
{
    public class DbProvider : IDbProvider
    {
        public const string DbConfigKey = "DB";
        private IConfig _Config;
        private IServiceProvider _ServiceProvider;
        public Dictionary<string, DbSql> SqlConfigs { get; protected set; }

        public DbProvider(IConfig config, IServiceProvider serviceProvider)
        {
            _Config = config;
            _ServiceProvider = serviceProvider;
            SqlConfigs = _Config.Get<DbConfig>(DbConfigKey)?.Sqls;
        }

        private DbSql GetDbSql(string commandName)
        {
            DbSql sql = null;
            SqlConfigs?.TryGetValue(commandName, out sql);
            return sql;
        }

        public IDbConnection GetConnection()
        {
            return _ServiceProvider.GetService<IDbConnection>();
        }

        public T Exec<T>(string commandName, object parameters, IDbTransaction transaction, CommandFlags flags, CancellationToken cancellationToken, Func<IDbConnection, CommandDefinition, T> func)
        {
            var conn = GetConnection();
            var sql = GetDbSql(commandName);
            conn.ConnectionString = sql.ConnectionString;
            var cmd = sql.ToCommandDefinition(parameters, transaction, flags, cancellationToken);
            return func(conn, cmd);
        }

        public IEnumerable<T> Query<T>(string commandName, object parameters = null, IDbTransaction transaction = null, CommandFlags flags = CommandFlags.Buffered, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Exec(commandName, parameters, transaction, flags, cancellationToken, (conn, cmd) => conn.Query<T>(cmd));
        }

        public SqlMapper.GridReader QueryMultiple(string commandName, object parameters = null, IDbTransaction transaction = null, CommandFlags flags = CommandFlags.Buffered, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Exec(commandName, parameters, transaction, flags, cancellationToken, (conn, cmd) => conn.QueryMultiple(cmd));
        }

        public int Execute(string commandName, object parameters = null, IDbTransaction transaction = null, CommandFlags flags = CommandFlags.Buffered, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Exec(commandName, parameters, transaction, flags, cancellationToken, (conn, cmd) => conn.Execute(cmd));
        }

        public IDataReader ExecuteReader(string commandName, object parameters = null, IDbTransaction transaction = null, CommandFlags flags = CommandFlags.Buffered, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Exec(commandName, parameters, transaction, flags, cancellationToken, (conn, cmd) => conn.ExecuteReader(cmd));
        }

        public T ExecuteScalar<T>(string commandName, object parameters = null, IDbTransaction transaction = null, CommandFlags flags = CommandFlags.Buffered, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Exec(commandName, parameters, transaction, flags, cancellationToken, (conn, cmd) => conn.ExecuteScalar<T>(cmd));
        }

        public Task<IEnumerable<T>> QueryAsync<T>(string commandName, object parameters = null, IDbTransaction transaction = null, CommandFlags flags = CommandFlags.Buffered, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Exec(commandName, parameters, transaction, flags, cancellationToken, (conn, cmd) => conn.QueryAsync<T>(cmd));
        }

        public Task<SqlMapper.GridReader> QueryMultipleAsync(string commandName, object parameters = null, IDbTransaction transaction = null, CommandFlags flags = CommandFlags.Buffered, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Exec(commandName, parameters, transaction, flags, cancellationToken, (conn, cmd) => conn.QueryMultipleAsync(cmd));
        }

        public Task<int> ExecuteAsync(string commandName, object parameters = null, IDbTransaction transaction = null, CommandFlags flags = CommandFlags.Buffered, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Exec(commandName, parameters, transaction, flags, cancellationToken, (conn, cmd) => conn.ExecuteAsync(cmd));
        }

        public Task<IDataReader> ExecuteReaderAsync(string commandName, object parameters = null, IDbTransaction transaction = null, CommandFlags flags = CommandFlags.Buffered, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Exec(commandName, parameters, transaction, flags, cancellationToken, (conn, cmd) => conn.ExecuteReaderAsync(cmd));
        }

        public Task<T> ExecuteScalarAsync<T>(string commandName, object parameters = null, IDbTransaction transaction = null, CommandFlags flags = CommandFlags.Buffered, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Exec(commandName, parameters, transaction, flags, cancellationToken, (conn, cmd) => conn.ExecuteScalarAsync<T>(cmd));
        }

        public void ExecuteBulkCopy<T>(string commandName, List<T> parameters)
        {
            ExecuteBulkCopyAsync<T>(commandName, parameters).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public Task ExecuteBulkCopyAsync<T>(string commandName, List<T> parameters)
        {
            return Exec(commandName, parameters, null, CommandFlags.Buffered, default(CancellationToken), async (conn, cmd) =>
            {
                var con = conn as SqlConnection;
                await con.OpenAsync();
                using (var sqlBulkCopy = new SqlBulkCopy(con))
                {
                    sqlBulkCopy.DestinationTableName = cmd.CommandText;
                    var reader = new BulkCopyDataReader<T>(parameters);
                    reader.ColumnMappings.ForEach(i => sqlBulkCopy.ColumnMappings.Add(i));
                    await sqlBulkCopy.WriteToServerAsync(reader);
                }
            });
        }

        public T QueryFirstOrDefault<T>(string commandName, object parameters = null, IDbTransaction transaction = null, CommandFlags flags = CommandFlags.Buffered, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Exec(commandName, parameters, transaction, flags, cancellationToken, (conn, cmd) => conn.QueryFirstOrDefault<T>(cmd));
        }

        public Task<dynamic> QueryFirstOrDefaultAsync(string commandName, object parameters = null, IDbTransaction transaction = null, CommandFlags flags = CommandFlags.Buffered, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Exec(commandName, parameters, transaction, flags, cancellationToken, (conn, cmd) => conn.QueryFirstOrDefaultAsync(cmd));
        }
    }
}