using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace Pandv.DataAccess
{
    public interface IDbProvider
    {
        T Exec<T>(string commandName, object parameters, IDbTransaction transaction, CommandFlags flags, CancellationToken cancellationToken, Func<IDbConnection, CommandDefinition, T> func);

        IEnumerable<T> Query<T>(string commandName, object parameters = null, IDbTransaction transaction = null, CommandFlags flags = CommandFlags.Buffered, CancellationToken cancellationToken = default(CancellationToken));

        GridReader QueryMultiple(string commandName, object parameters = null, IDbTransaction transaction = null, CommandFlags flags = CommandFlags.Buffered, CancellationToken cancellationToken = default(CancellationToken));

        int Execute(string commandName, object parameters = null, IDbTransaction transaction = null, CommandFlags flags = CommandFlags.Buffered, CancellationToken cancellationToken = default(CancellationToken));

        IDataReader ExecuteReader(string commandName, object parameters = null, IDbTransaction transaction = null, CommandFlags flags = CommandFlags.Buffered, CancellationToken cancellationToken = default(CancellationToken));

        T ExecuteScalar<T>(string commandName, object parameters = null, IDbTransaction transaction = null, CommandFlags flags = CommandFlags.Buffered, CancellationToken cancellationToken = default(CancellationToken));

        Task<IEnumerable<T>> QueryAsync<T>(string commandName, object parameters = null, IDbTransaction transaction = null, CommandFlags flags = CommandFlags.Buffered, CancellationToken cancellationToken = default(CancellationToken));

        Task<GridReader> QueryMultipleAsync(string commandName, object parameters = null, IDbTransaction transaction = null, CommandFlags flags = CommandFlags.Buffered, CancellationToken cancellationToken = default(CancellationToken));

        Task<int> ExecuteAsync(string commandName, object parameters = null, IDbTransaction transaction = null, CommandFlags flags = CommandFlags.Buffered, CancellationToken cancellationToken = default(CancellationToken));

        Task<IDataReader> ExecuteReaderAsync(string commandName, object parameters = null, IDbTransaction transaction = null, CommandFlags flags = CommandFlags.Buffered, CancellationToken cancellationToken = default(CancellationToken));

        Task<T> ExecuteScalarAsync<T>(string commandName, object parameters = null, IDbTransaction transaction = null, CommandFlags flags = CommandFlags.Buffered, CancellationToken cancellationToken = default(CancellationToken));

        void ExecuteBulkCopy<T>(string commandName, List<T> parameters);

        Task ExecuteBulkCopyAsync<T>(string commandName, List<T> parameters);

        T QueryFirstOrDefault<T>(string commandName, object parameters = null, IDbTransaction transaction = null, CommandFlags flags = CommandFlags.Buffered, CancellationToken cancellationToken = default(CancellationToken));

        Task<dynamic> QueryFirstOrDefaultAsync(string commandName, object parameters = null, IDbTransaction transaction = null, CommandFlags flags = CommandFlags.Buffered, CancellationToken cancellationToken = default(CancellationToken));
    }
}