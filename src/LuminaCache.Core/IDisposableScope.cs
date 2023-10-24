namespace LuminaCache.Core;

public interface IDisposableScope : IDisposable
{
    T GetService<T>() where T : class;
}