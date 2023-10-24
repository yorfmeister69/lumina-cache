# Lumina Cache 💡

**Lumina Cache** is a high-performance, in-memory caching solution tailored for modern .NET applications. Drawing inspiration from the brilliance and speed of light (`Lumina` is Latin for light), our goal is to ensure that your application's data access is as fast and efficient as photons racing through the void.

## 🌟 Features

- **Type-safe Property Indexing**: Lumina Cache uses lambda expressions, ensuring type-safety and reducing potential runtime errors.
- **Dynamic Cache Refreshing**: Keep your cache up-to-date with minimal effort, maximizing data accuracy and relevance.
- **Robust Multi-threading**: With built-in safeguards, Lumina Cache is ready for multi-threaded environments, ensuring consistent and reliable performance.
- **Extensible Design**: Designed with the future in mind, Lumina Cache is ready to integrate with additional storage mechanisms and strategies.

## Setup

### Add Lumina Cache to your project

```csharp
builder.Services.AddLuminaCache();
```
This will return a `LuminaBuilder` instance, which can be used to configure the cache.

#### Add cache object collection
```csharp
builder.Services.AddLuminaCache()
    .AddCollection<TObject, TPrimaryKey>(x => x.Id, config =>
    {
        config.SetExpiration(TimeSpan.FromHours(1));
        config.AddSecondaryIndex(x => x.SecondaryIndex);
    });
```
* **AddCollection<TObject, TPrimaryKey>:** This is where you define a cache collection for a specific type, TObject. Every object of this type will have its caching behavior managed according to this configuration.

* **Primary Key (x => x.Id)**: Every cached item needs a unique identifier, or a primary key. Here, the Id property of the TObject is chosen as that unique identifier.

* **Configuration (inside config):**

    * **Expiration (SetExpiration(TimeSpan.FromHours(1)))**: Items in the cache don't stay there forever. Here, we've specified that items should expire after 1 hour. After this time, they're automatically refreshed from the cache using the Backing store.
    * **Secondary Index (AddSecondaryIndex(x => x.SecondaryIndex))**: While every item has a primary key, sometimes it's useful to fetch items based on other properties. This method allows you to define additional indices for quicker access. In this case, we're indexing items by a SecondaryIndex property.

#### Specifying the Backing Store
```csharp
builder.Services.AddLuminaCache()
    .AddCollection<TObject, TPrimaryKey>(x => x.Id, config =>
    {
        config.SetExpiration(TimeSpan.FromHours(1));
        config.AddSecondaryIndex(x => x.SecondaryIndex);
    }).AddBackingStore<TObjectBackingStore>();
```

When the cache expires, it needs to be refreshed from the backing store. Here, we've specified that the cache should use the `TObjectBackingStore` class to refresh the cache.

#### Add multiple collections and backing stores as needed.
```csharp
builder.Services.AddLuminaCache()
    .AddCollection<TObject, TPrimaryKey>(x => x.Id, config =>
    {
        config.SetExpiration(TimeSpan.FromHours(1));
        config.AddSecondaryIndex(x => x.SecondaryIndex);
    }).AddBackingStore<TObjectBackingStore>()
    
    .AddCollection<TOtherObject, TPrimaryKey>(x => x.Id, config =>
    {
        config.SetExpiration(TimeSpan.FromHours(1));
        config.AddSecondaryIndex(x => x.SecondaryIndex);
    }).AddBackingStore<TOtherObjectBackingStore>();
```

### Basic Usage

When a collection is added, a client is automatically generated for that collection. This client is a singleton class that can be injected into any class that needs to access the cache.

```csharp
ICacheClient<TObject, TPrimaryKey>
```

To use the cache client in your classes (e.g., controllers or services), inject it via the constructor:
```csharp
private readonly ICacheClient<MyClass, Guid> _cacheClient;

public MyService(ICacheClient<MyClass, Guid> cacheClient)
{
    _cacheClient = cacheClient;
}

```

#### Fetching a Single Item
If you know the primary key:
```csharp
var item = await _cacheClient.GetAsync(myPrimaryKey);
```

#### Fetching Multiple Items
Using a list of primary keys:
```csharp
var items = await _cacheClient.GetManyAsync(myListOfPrimaryKeys);
```

#### Fetching by Secondary Index
If you have indexed properties and want to fetch items by them:
```csharp
var itemsByIndex = await _cacheClient.GetByIndexAsync(x => x.SomeIndexedProperty, someValue);
```

#### Executing Custom Queries
Formulate custom LINQ queries on your cached data:
```csharp
var query = _cacheClient.Query().Where(x => x.SomeProperty == someValue);
var result = await _cacheClient.MaterializeAsync(query);
```