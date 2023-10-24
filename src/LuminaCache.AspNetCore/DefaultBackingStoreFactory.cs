using LuminaCache.Core;

namespace LuminaCache.AspNetCore;

public class DefaultBackingStoreFactory : IBackingStoreFactory
{
    private readonly IScopeFactory _scopeFactory;

    public DefaultBackingStoreFactory(IScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }
    public ScopedBackingStoreContainer<T> Create<T>() where T : class
    {
        var scope = _scopeFactory.CreateScope();

        var service = scope.GetService<IBackingStore<T>>();

        return new ScopedBackingStoreContainer<T>(scope, service);
    }
}