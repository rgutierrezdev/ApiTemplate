namespace ApiTemplate.Application.Common.Cache;

public interface ICacheKeyService : IScopedService
{
    public string GetKey(string cacheKey, params string[] paramValues);
}
