namespace ApiTemplate.Application.Common.Persistence;

public interface IRepository<T> where T : class, IEntity
{
    #region Modification Operations ------------------------------------------------------------------------------------

    void Add(T entity);

    void AddRange(IEnumerable<T> entities);

    void Update(T entity);

    void UpdateRange(IEnumerable<T> entities);

    void Delete(T entity);

    void DeleteRange(IEnumerable<T> entities);

    #endregion

    #region Read Operations --------------------------------------------------------------------------------------------

    Task<T?> GetByIdAsync<TId>(TId id, CancellationToken cancellationToken = default) where TId : notnull;

    Task<T?> FirstOrDefaultAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);

    Task<TResult?> FirstOrDefaultAsync<TResult>(
        ISpecification<T, TResult> specification,
        CancellationToken cancellationToken = default
    );

    Task<T?> FirstOrDefaultAsync(
        Action<ISpecificationBuilder<T>> query,
        CancellationToken cancellationToken = default
    )
    {
        return FirstOrDefaultAsync(new CustomSpecification<T>(query), cancellationToken);
    }

    Task<TResult?> FirstOrDefaultAsync<TResult>(
        Action<ISpecificationBuilder<T, TResult>> query,
        CancellationToken cancellationToken = default
    )
    {
        return FirstOrDefaultAsync(new CustomSpecification<T, TResult>(query), cancellationToken);
    }

    Task<List<T>> ListAsync(CancellationToken cancellationToken = default);

    Task<List<T>> ListAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);

    Task<List<TResult>> ListAsync<TResult>(
        ISpecification<T, TResult> specification,
        CancellationToken cancellationToken = default
    );

    Task<List<T>> ListAsync(
        Action<ISpecificationBuilder<T>> query,
        CancellationToken cancellationToken = default
    )
    {
        return ListAsync(new CustomSpecification<T>(query), cancellationToken);
    }

    Task<List<TResult>> ListAsync<TResult>(
        Action<ISpecificationBuilder<T, TResult>> query,
        CancellationToken cancellationToken = default
    )
    {
        return ListAsync(new CustomSpecification<T, TResult>(query), cancellationToken);
    }

    Task<int> CountAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);

    Task<int> CountAsync(Action<ISpecificationBuilder<T>> query, CancellationToken cancellationToken = default)
    {
        return CountAsync(new CustomSpecification<T>(query), cancellationToken);
    }

    Task<int> CountAsync(CancellationToken cancellationToken = default);

    Task<bool> AnyAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);

    Task<bool> AnyAsync(Action<ISpecificationBuilder<T>> query, CancellationToken cancellationToken = default)
    {
        return AnyAsync(new CustomSpecification<T>(query), cancellationToken);
    }

    Task<bool> AnyAsync(CancellationToken cancellationToken = default);

    #endregion
}
