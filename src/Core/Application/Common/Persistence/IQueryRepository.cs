using System.Data;
using SqlKata;

namespace ApiTemplate.Application.Common.Persistence;

public interface IQueryRepository
{
    bool Exists(Query query, IDbTransaction? transaction = null, int? timeout = null);

    Task<bool> ExistsAsync(
        Query query,
        IDbTransaction? transaction = null,
        int? timeout = null,
        CancellationToken cancellationToken = default
    );

    Task<bool> ExistsAsync(
        Query query,
        CancellationToken cancellationToken = default
    )
    {
        return ExistsAsync(query, null, null, cancellationToken);
    }

    IEnumerable<T> Get<T>(Query query, IDbTransaction? transaction = null, int? timeout = null);

    Task<IEnumerable<T>> GetAsync<T>(
        Query query,
        IDbTransaction? transaction = null,
        int? timeout = null,
        CancellationToken cancellationToken = default
    );

    Task<IEnumerable<T>> GetAsync<T>(
        Query query,
        CancellationToken cancellationToken = default
    )
    {
        return GetAsync<T>(query, null, null, cancellationToken);
    }

    T FirstOrDefault<T>(Query query, IDbTransaction? transaction = null, int? timeout = null);

    Task<T> FirstOrDefaultAsync<T>(
        Query query,
        IDbTransaction? transaction = null,
        int? timeout = null,
        CancellationToken cancellationToken = default
    );

    Task<T> FirstOrDefaultAsync<T>(
        Query query,
        CancellationToken cancellationToken = default
    )
    {
        return FirstOrDefaultAsync<T>(query, null, null, cancellationToken);
    }

    T First<T>(Query query, IDbTransaction? transaction = null, int? timeout = null);

    Task<T> FirstAsync<T>(
        Query query,
        IDbTransaction? transaction = null,
        int? timeout = null,
        CancellationToken cancellationToken = default
    );

    Task<T> FirstAsync<T>(
        Query query,
        CancellationToken cancellationToken = default
    )
    {
        return FirstAsync<T>(query, null, null, cancellationToken);
    }

    T Scalar<T>(Query query, IDbTransaction? transaction = null, int? timeout = null);

    Task<T> ScalarAsync<T>(
        Query query,
        IDbTransaction? transaction = null,
        int? timeout = null,
        CancellationToken cancellationToken = default
    );

    Task<T> ScalarAsync<T>(
        Query query,
        CancellationToken cancellationToken = default
    )
    {
        return ScalarAsync<T>(query, null, null, cancellationToken);
    }
}
