using System.Linq.Expressions;

namespace LuminaCache.Core;

public interface ICacheClient<T, TPrimaryKey> where T : class where TPrimaryKey : notnull
{
     Task<T?> GetAsync(TPrimaryKey key, CancellationToken cancellationToken = default);
     Task<Dictionary<TPrimaryKey, T?>> GetManyAsync(IEnumerable<TPrimaryKey> keys, CancellationToken cancellationToken = default); 
     Task<IEnumerable<T>> GetByIndexAsync<TIndex>(Expression<Func<T, TIndex>> indexSelector, TIndex indexValue, CancellationToken cancellationToken = default) where TIndex : notnull;
     Task<List<T>> MaterializeAsync(IQueryable<T> queryable, CancellationToken cancellationToken = default);
     IQueryable<T> Query();
}