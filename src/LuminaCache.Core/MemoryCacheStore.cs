using System.Linq.Expressions;

namespace LuminaCache.Core;

/// <summary>
/// A generic memory cache store. (Singleton class)
/// </summary>
/// <typeparam name="T">type of cache object</typeparam>
/// <typeparam name="TPrimaryKey">type of cache object primary key</typeparam>
public class MemoryCacheStore<T, TPrimaryKey> : IMemoryCacheStore<T, TPrimaryKey> where T : class where TPrimaryKey : notnull
{
    private readonly List<T> _items = new();
    private Dictionary<TPrimaryKey, T> _primaryIndex = new();
    private Dictionary<string, Dictionary<object, List<T>>> _indices = new();
    private readonly MemoryCacheStoreConfig<T, TPrimaryKey> _config;
    
    private DateTimeOffset _lastUpdatedOn = DateTimeOffset.MinValue;

    public MemoryCacheStore(MemoryCacheStoreConfig<T, TPrimaryKey>  config)
    {
        _config = config;
    }

    public void Build(IEnumerable<T> items)
    {
        //add items
        var collection = items as T[] ?? items.ToArray();
        _items.Clear();
        _items.AddRange(collection);

        //build primary index
        _primaryIndex = _items.ToDictionary(_config.PrimaryKeySelector);

        //build secondary index
        if (_config.SecondaryIndexSelectors.Any())
        {
            _indices = BuildIndices(collection, _config.SecondaryIndexSelectors);
        }
        
        _lastUpdatedOn = DateTimeOffset.UtcNow;
    }
    
    public T? Get(TPrimaryKey key)
    {
        return _primaryIndex.TryGetValue(key, out var item) ? item : null;
    }
    
    public Dictionary<TPrimaryKey, T?> GetMany(IEnumerable<TPrimaryKey> keys)
    {
        keys = keys.Distinct();
        return keys.ToDictionary(key => key, Get);
    }
    
    public IEnumerable<T> GetByIndex(string indexName, object indexValue)
    {
        if (!_indices.TryGetValue(indexName, out var index))
        {
            throw new InvalidOperationException($"Index {indexName} does not exist");
        }

        return index.TryGetValue(indexValue, out var items) ? items : Enumerable.Empty<T>();
    }
    
    public IQueryable<T> Query()
    {
        return _items.AsQueryable();
    }

    public bool IsExpired()
    {
        if(_config.Expiration is null) return false;
        
        return DateTimeOffset.UtcNow - _lastUpdatedOn > _config.Expiration;
    }

    private static Dictionary<string, Dictionary<object, List<T>>> BuildIndices(
        T[] items,
        Dictionary<string, Func<T, object>> keySelectors)
    {
        var indices = new Dictionary<string, Dictionary<object, List<T>>>();

        foreach (var keySelector in keySelectors)
        {
            indices[keySelector.Key] = items
                .GroupBy(keySelector.Value)
                .ToDictionary(group => group.Key, group => group.ToList());
        }

        return indices;
    }
}

public class MemoryCacheStoreConfig<T, TPrimaryKey> where T : class where TPrimaryKey : notnull
{
    public MemoryCacheStoreConfig(Func<T, TPrimaryKey> primaryKeySelector)
    {
        PrimaryKeySelector = primaryKeySelector;
    }

    public Func<T, TPrimaryKey> PrimaryKeySelector { get; private set; }
    public Dictionary<string, Func<T, object>> SecondaryIndexSelectors { get; private set; } = new();
    
    public TimeSpan? Expiration { get; private set; }
    
    /// <summary>
    /// Add a secondary index to the cache store.
    /// </summary>
    /// <param name="indexSelector"></param>
    public void AddSecondaryIndex(Expression<Func<T, object>> indexSelector)
    {
        var indexName = indexSelector.GetMemberName();
        SecondaryIndexSelectors[indexName] = indexSelector.Compile();
    }
    
    /// <summary>
    /// Set the expiration of the cache store. After this period, the cache will be considered expired and will be rebuilt using the backing store.
    /// Cache will be rebuilt only when a request is made to the cache.
    /// </summary>
    /// <param name="expiration"></param>
    public void SetExpiration(TimeSpan expiration)
    {
        Expiration = expiration;
    }
}