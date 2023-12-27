using System.Text;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using ApiTemplate.Application.Common.Cache;
using ApiTemplate.Application.Common.Interfaces;

namespace ApiTemplate.Infrastructure.Caching;

public class DistributedCacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<DistributedCacheService> _logger;
    private readonly ISerializerService _serializer;

    public DistributedCacheService(
        IDistributedCache cache,
        ISerializerService serializer,
        ILogger<DistributedCacheService> logger
    )
    {
        _cache = cache;
        _serializer = serializer;
        _logger = logger;
    }

    public T? Get<T>(string key)
    {
        return Get(key) is { } data
            ? Deserialize<T>(data)
            : default;
    }

    private byte[]? Get(string key)
    {
        ArgumentNullException.ThrowIfNull(key);

        try
        {
            return _cache.Get(key);
        }
        catch
        {
            return null;
        }
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken token = default)
    {
        return await GetAsync(key, token) is { } data
            ? Deserialize<T>(data)
            : default;
    }

    private async Task<byte[]?> GetAsync(string key, CancellationToken token = default)
    {
        try
        {
            return await _cache.GetAsync(key, token);
        }
        catch
        {
            return null;
        }
    }

    public void Remove(string key)
    {
        try
        {
            _cache.Remove(key);
        }
        catch
        {
        }
    }

    public async Task RemoveAsync(string key, CancellationToken token = default)
    {
        try
        {
            await _cache.RemoveAsync(key, token);
        }
        catch
        {
        }
    }

    public void Set<T>(string key, T value, TimeSpan? slidingExpiration = null)
    {
        Set(key, Serialize(value), slidingExpiration);
    }

    private void Set(string key, byte[] value, TimeSpan? slidingExpiration = null)
    {
        try
        {
            _cache.Set(key, value, GetOptions(slidingExpiration));
            _logger.LogDebug("Added to Cache : {Key}", key);
        }
        catch
        {
        }
    }

    public Task SetAsync<T>(
        string key,
        T value,
        TimeSpan? slidingExpiration = null,
        CancellationToken cancellationToken = default
    )
    {
        return SetAsync(key, Serialize(value), slidingExpiration, cancellationToken);
    }

    private async Task SetAsync(
        string key,
        byte[] value,
        TimeSpan? slidingExpiration = null,
        CancellationToken token = default
    )
    {
        try
        {
            await _cache.SetAsync(key, value, GetOptions(slidingExpiration), token);
            _logger.LogDebug("Added to Cache : {Key}", key);
        }
        catch
        {
        }
    }

    private byte[] Serialize<T>(T item)
    {
        return Encoding.Default.GetBytes(_serializer.Serialize(item));
    }

    private T Deserialize<T>(byte[] cachedData)
    {
        return _serializer.Deserialize<T>(Encoding.Default.GetString(cachedData))!;
    }

    private static DistributedCacheEntryOptions GetOptions(TimeSpan? slidingExpiration)
    {
        var options = new DistributedCacheEntryOptions();

        if (slidingExpiration.HasValue)
        {
            options.SetSlidingExpiration(slidingExpiration.Value);
        }

        return options;
    }
}
