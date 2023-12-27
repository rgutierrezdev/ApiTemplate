# Caching

Caching is one of the best options to improve performance of your apps by simply caching data instead of always getting
the data from the original source.

See [Cache configuration](configurations.md#cache)

To cache a value you need to inject first the `ICacheService` service.

```csharp
    private readonly ICacheService _cacheService;

    public Handler(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }
```

And then you just have to set the value for an specific key using the `Set` or `SetAsync` functions and `Get`
or `GetAsync` functions to get the value for it:

```csharp
public async Task Handle(
    Request request,
    CancellationToken cancellationToken
)
{
    // generate the cache key
    var cacheKey = $"user_{_currentUser.Id}";
    
    // getting the cached value
    var cached = await _cacheService.GetAsync<User>(cacheKey, cancellationToken);

    if (cached != null)
        return cached;

    var user = await _repository.ListAsync<User>(
        q => q.Where(u => u.Id == _currentUser.Id),
        cancellationToken
    );

    // setting the cache value
    await _cacheService.SetAsync(cacheKey, user, null, cancellationToken);

    return documentTypes;
}
```

If you want to avoid typos when using the same cache key multiple times across different files it is recommended to use
the `ICacheKeyService` to generate the cache keys along with `CacheKeys` class to keep track of all the cache key
templates and avoid conflicts between developers.
This is the recommended way to always generate cache keys.

```csharp
public static class CacheKeys
{
    public const string User = "user_[userId]";
    public const string UserDocumentTypes = "DocumentTypes_[tenantId]_[userId]";
    ...
}
```

You can also define keys with multiple parameters, when generating its key you just need to pass the parameters in
order of definition in the key template:

```csharp
var cacheKey = _cacheKeyService.GetKey(UserDocumentTypes.User, _currentUser.TenantId.ToString(), _currentUser.Id.ToString());
```

Let´s see the previous example using the cache key generator service.

```csharp
private readonly ICacheService _cacheService;
private readonly ICacheKeyService _cacheKeyService;

public Handler(ICacheService cacheService, ICacheKeyService cacheKeyService)
{
    _cacheService = cacheService;
    _cacheKeyService = cacheKeyService;
}

public async Task Handle(
    Request request,
    CancellationToken cancellationToken
)
{
    // generate the cache key
    var cacheKey = _cacheKeyService.GetKey(CacheKeys.User, _currentUser.Id.ToString());
    
    ...

    return documentTypes;
}
```
