namespace LuminaCache.Core;

/// <summary>
/// The backing store is the source of truth for the data.  It is
/// responsible for providing a list of items to be cached.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IBackingStore<T> where T : class
{
    /// <summary>
    /// Get the items to be cached
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IEnumerable<T>> GetItemsAsync(CancellationToken cancellationToken = default);
}