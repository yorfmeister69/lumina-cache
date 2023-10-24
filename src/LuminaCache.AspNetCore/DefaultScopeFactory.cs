using LuminaCache.Core;
using Microsoft.Extensions.DependencyInjection;

namespace LuminaCache.AspNetCore;

public class DefaultScopeFactory: IScopeFactory
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public DefaultScopeFactory(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }
    public IDisposableScope CreateScope()
    {
        var scope = _serviceScopeFactory.CreateScope();
        return new DefaultScope(scope);
    }
}