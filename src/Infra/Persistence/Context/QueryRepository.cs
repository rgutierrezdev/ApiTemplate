using System.Data;
using ApiTemplate.Application.Common.Persistence;
using SqlKata;
using SqlKata.Execution;

namespace ApiTemplate.Persistence.Context;

public class QueryRepository : IQueryRepository
{
    private readonly QueryFactory _queryFactory;

    public QueryRepository(QueryFactory queryFactory)
    {
        _queryFactory = queryFactory;
    }

    public bool Exists(Query query, IDbTransaction? transaction = null, int? timeout = null)
    {
        return _queryFactory.Exists(query, transaction, timeout);
    }

    public async Task<bool> ExistsAsync(
        Query query,
        IDbTransaction? transaction = null,
        int? timeout = null,
        CancellationToken cancellationToken = default
    )
    {
        return await _queryFactory.ExistsAsync(query, transaction, timeout, cancellationToken);
    }

    public IEnumerable<T> Get<T>(Query query, IDbTransaction? transaction = null, int? timeout = null)
    {
        return _queryFactory.Get<T>(query, transaction, timeout);
    }

    public async Task<IEnumerable<T>> GetAsync<T>(
        Query query,
        IDbTransaction? transaction = null,
        int? timeout = null,
        CancellationToken cancellationToken = default
    )
    {
        return await _queryFactory.GetAsync<T>(query, transaction, timeout, cancellationToken);
    }

    public T FirstOrDefault<T>(Query query, IDbTransaction? transaction = null, int? timeout = null)
    {
        return _queryFactory.FirstOrDefault<T>(query, transaction, timeout);
    }

    public async Task<T> FirstOrDefaultAsync<T>(
        Query query,
        IDbTransaction? transaction = null,
        int? timeout = null,
        CancellationToken cancellationToken = default
    )
    {
        return await _queryFactory.FirstOrDefaultAsync<T>(query, transaction, timeout, cancellationToken);
    }

    public T First<T>(Query query, IDbTransaction? transaction = null, int? timeout = null)
    {
        return _queryFactory.First<T>(query, transaction, timeout);
    }

    public async Task<T> FirstAsync<T>(
        Query query,
        IDbTransaction? transaction = null,
        int? timeout = null,
        CancellationToken cancellationToken = default
    )
    {
        return await _queryFactory.FirstAsync<T>(query, transaction, timeout, cancellationToken);
    }

    public T Scalar<T>(Query query, IDbTransaction? transaction = null, int? timeout = null)
    {
        return _queryFactory.ExecuteScalar<T>(query, transaction, timeout);
    }

    public async Task<T> ScalarAsync<T>(
        Query query,
        IDbTransaction? transaction = null,
        int? timeout = null,
        CancellationToken cancellationToken = default
    )
    {
        return await _queryFactory.ExecuteScalarAsync<T>(query, transaction, timeout, cancellationToken);
    }
}
