namespace LuminaCache.Core;

/// <summary>
/// Backing store factory creator.  This is used to create a <see cref="ScopedBackingStoreContainer{T}"/>
/// </summary>
public interface IBackingStoreFactory 
{
     /// <summary>
     /// Creates a <see cref="ScopedBackingStoreContainer{T}"/> for the specified type
     /// </summary>
     ScopedBackingStoreContainer<T> Create<T>() where T : class;
}