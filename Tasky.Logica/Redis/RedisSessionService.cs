using Microsoft.Extensions.Caching.Distributed;


namespace Tasky.Logica.Redis;
public interface IRedisSessionService
{
    Task SetValueAsync(string key, string value);
    Task<string> GetValueAsync(string key);
    Task RemoveValueAsync(string key);
}

public class RedisSessionService : IRedisSessionService
{
    private readonly IDistributedCache _distributedCache;

    public RedisSessionService(IDistributedCache distributedCache)
    {
        _distributedCache = distributedCache;
    }

    public async Task SetValueAsync(string key, string value)
    {
        await _distributedCache.SetStringAsync(key, value);
    }

    public async Task<string> GetValueAsync(string key)
    {
        return await _distributedCache.GetStringAsync(key);
    }

    public async Task RemoveValueAsync(string key)
    {
        await _distributedCache.RemoveAsync(key);
    }
}

