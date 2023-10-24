using LuminaCache.Core;
using Microsoft.Extensions.DependencyInjection;

namespace LuminaCache.AspNetCore;

public class DefaultScope : IDisposableScope
{
    private readonly IServiceScope _scope;

    public DefaultScope(IServiceScope scope)
    {
        _scope = scope;
    }
    public void Dispose()
    {
        _scope.Dispose();
    }

    public T GetService<T>() where T : class
    {
        return _scope.ServiceProvider.GetRequiredService<T>();
    }
}