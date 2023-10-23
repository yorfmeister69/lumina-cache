using System.Linq.Expressions;

namespace LuminaCache.Core;

/// <summary>
/// A generic memory cache client. (Scoped class)
/// </summary>
public class MemoryCacheClient<T, TPrimaryKey> : ICacheClient<T, TPrimaryKey> where T : class where TPrimaryKey : notnull
{
    private readonly IMemoryCacheStore<T, TPrimaryKey> _cacheStore;
    private readonly IBackingStoreFactory _backingStoreFactory;
    private readonly SemaphoreSlim _semaphore = new(1, 1); 

    public MemoryCacheClient(IMemoryCacheStore<T, TPrimaryKey> cacheStore, IBackingStoreFactory backingStoreFactory)
    {
        _cacheStore = cacheStore;
        _backingStoreFactory = backingStoreFactory;
    }


    public async Task<T?> GetAsync(TPrimaryKey key, CancellationToken cancellationToken = default)
    {
        await EnsureCacheUpdated(cancellationToken);
        return _cacheStore.Get(key);
    }

    public async Task<Dictionary<TPrimaryKey, T?>> GetManyAsync(IEnumerable<TPrimaryKey> keys, CancellationToken cancellationToken = default)
    {
        await EnsureCacheUpdated(cancellationToken);
        return _cacheStore.GetMany(keys);
    }

    public async Task<IEnumerable<T>> GetByIndexAsync<TIndex>(Expression<Func<T, TIndex>> indexSelector, TIndex indexValue, CancellationToken cancellationToken = default) where TIndex : notnull
    {
        await EnsureCacheUpdated(cancellationToken);
        var indexName = indexSelector.GetMemberName();
        return _cacheStore.GetByIndex(indexName, indexValue);
    }

    public IQueryable<T> Query() => _cacheStore.Query();
    
    public async Task<List<T>> MaterializeAsync(IQueryable<T> queryable, CancellationToken cancellationToken = default)
    {
        await EnsureCacheUpdated(cancellationToken);
        return queryable.ToList();
    }

    private async Task EnsureCacheUpdated(CancellationToken cancellationToken)
    {
        //if cache is not expired, return.
        if (!_cacheStore.IsExpired())
            return;
        
        //block threads from updating the cache concurrently.
        await _semaphore.WaitAsync(cancellationToken);
        
        try
        {
            //check again if cache is not expired, return.
            if (!_cacheStore.IsExpired())
                return;
            
            //get the backing store
            using var storeContainer = _backingStoreFactory.Create<T>();
            var store = storeContainer.Store;
            var items = await store.GetItemsAsync(cancellationToken);
            
            //build the cache
            _cacheStore.Build(items);
            
        }
        finally
        {
            _semaphore.Release();
        }
    }
}