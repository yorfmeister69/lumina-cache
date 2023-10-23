namespace LuminaCache.Core;

public interface IScopeFactory
{
    IDisposableScope CreateScope();
}