namespace LuminaCache.Core;

public interface IMemoryCacheStore<T, TPrimaryKey> where T : class where TPrimaryKey : notnull
{
    void Build(IEnumerable<T> items);
    T? Get(TPrimaryKey key);
    Dictionary<TPrimaryKey, T?> GetMany(IEnumerable<TPrimaryKey> keys);
    IEnumerable<T> GetByIndex(string indexName, object indexValue);
    IQueryable<T> Query();
    
    bool IsExpired();
}