namespace LuminaCache.Core;

/// <summary>
/// Container for <see cref="IDisposableScope"/> and <see cref="IBackingStore{T}"/>
/// </summary>
/// <typeparam name="T"></typeparam>
public class ScopedBackingStoreContainer<T> : IDisposable where T : class
{
    /// <summary>
    /// Scope for the backing store
    /// </summary>
    public IDisposableScope Scope { get; }
    
    /// <summary>
    /// Backing store
    /// </summary>
    public IBackingStore<T> Store { get; }
    
    
    public ScopedBackingStoreContainer(IDisposableScope scope, IBackingStore<T> store)
    {
        Scope = scope;
        Store = store;
    }
    
    public void Dispose()
    {
        (Store as IDisposable)?.Dispose();
        Scope.Dispose();
    }
}