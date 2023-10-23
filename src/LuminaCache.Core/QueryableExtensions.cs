namespace LuminaCache.Core;

public static class QueryableExtensions
{
    /// <summary>
    /// Efficiently converts an <see cref="IQueryable{T}"/> sequence into a list of items of type <typeparamref name="T"/> 
    /// by interfacing with an <see cref="ICacheClient{T, TPrimaryKey}"/>.
    /// This method ensures that the underlying cache remains valid and not expired during the materialization process.
    /// </summary>
    /// <typeparam name="T">The type of the items in the queryable sequence.</typeparam>
    /// <typeparam name="TPrimaryKey">The type of the primary key used by the cache client.</typeparam>
    /// <param name="queryable">The IQueryable sequence to be materialized.</param>
    /// <param name="client">The cache client used to ensure data validity and non-expiry.</param>
    /// <param name="cancellationToken">An optional token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation, containing a list of items of type <typeparamref name="T"/>.</returns>
    public static async Task<List<T>> MaterializeAsync<T, TPrimaryKey>(this IQueryable<T> queryable, ICacheClient<T, TPrimaryKey> client, CancellationToken cancellationToken = default) where T : class where TPrimaryKey : notnull
    {
        return await client.MaterializeAsync(queryable, cancellationToken);
    }
}