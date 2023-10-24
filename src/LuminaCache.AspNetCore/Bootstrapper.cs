using System.Linq.Expressions;
using LuminaCache.Core;
using Microsoft.Extensions.DependencyInjection;

namespace LuminaCache.AspNetCore;

public class BackingStoreBuilder<T> where T : class
{
    private readonly LuminaBuilder _luminaBuilder;
    public BackingStoreBuilder(LuminaBuilder luminaBuilder)
    {
        _luminaBuilder = luminaBuilder;
    }
    
    /// <summary>
    /// Adds the specified IBackingStore implementation. The implementation will be registered as a scoped service and will be resolved per cache rebuild request by the IBackingStoreFactory in that scope. Will be disposed after the cache rebuild is complete.
    /// </summary>
    /// <typeparam name="T">Type of Cache entity</typeparam>
    /// <typeparam name="TStore">Type of BackingStore</typeparam>
    public LuminaBuilder AddBackingStore<TStore>() where TStore : class, IBackingStore<T>
    {
        _luminaBuilder.Services.AddScoped<IBackingStore<T>, TStore>();
        return _luminaBuilder;
    }
}

public class LuminaBuilder
{
    public readonly IServiceCollection Services;

    public LuminaBuilder(IServiceCollection services)
    {
        Services = services;
    }

    public BackingStoreBuilder<T> AddCollection<T, TKey>(Expression<Func<T, TKey>> primaryKeySelector, Action<MemoryCacheStoreConfig<T, TKey>> configureOptions) where T : class where TKey : notnull
    {
        var config = new MemoryCacheStoreConfig<T, TKey>(primaryKeySelector.Compile());
        
        configureOptions(config);
        
        Services.AddSingleton<IMemoryCacheStore<T, TKey>>(
            new MemoryCacheStore<T, TKey>(config));
        
        Services.AddSingleton<ICacheClient<T, TKey>, MemoryCacheClient<T, TKey>>();
        return new BackingStoreBuilder<T>(this);
    }
}

public static class Bootstrapper
{
    public static LuminaBuilder AddLuminaCache(this IServiceCollection services)
    {
        services.AddSingleton<IScopeFactory, DefaultScopeFactory>();
        services.AddSingleton<IBackingStoreFactory, DefaultBackingStoreFactory>();

        return new LuminaBuilder(services);
    }
    
    public static LuminaBuilder AddLuminaCache<TScopeFactory, TBackingStoreFactory>(this IServiceCollection services) 
        where TScopeFactory : class, IScopeFactory 
        where TBackingStoreFactory : class, IBackingStoreFactory
    {
        services.AddSingleton<IScopeFactory, TScopeFactory>();
        services.AddSingleton<IBackingStoreFactory, TBackingStoreFactory>();

        return new LuminaBuilder(services);
    }
}