using System.Text.Json;
using UserManagement.Application.Common.Interfaces;


namespace UserManagement.Infrastructure.Redis;
public class RedisCacheService : IRedisCacheService
{
    public async Task<T> GetOrSetCacheValueAsync<T>(string key, Func<Task<T>> fetchDataAsync)
    {
        var redisPool = RedisConnectionPool.Instance;

        // Get a connection from the pool
        var connection = redisPool.GetConnection();
        try
        {
            var db = connection.GetDatabase();

            // Try to get the cached value
            var cachedValue = await db.StringGetAsync(key);
            if (!cachedValue.IsNullOrEmpty)
            {
                // Deserialize the cached value to the expected type
                return JsonSerializer.Deserialize<T>(cachedValue);
            }

            // If not in cache, fetch data and cache it
            var newValue = await fetchDataAsync();

            // Serialize the data before caching
            var serializedValue = JsonSerializer.Serialize<T>(newValue);
            await db.StringSetAsync(key, serializedValue);

            return newValue;
        }
        catch (Exception ex)
        {
            // Log the exception and fallback to fetch data directly
            Console.WriteLine($"Redis error on GetOrSetCacheValueAsync for key '{key}': {ex.Message}");
            return await fetchDataAsync();
        }
        finally
        {
            // Return the connection to the pool
            redisPool.ReturnConnection(connection);
        }
    }

    public async Task RemoveCacheAsync(string key)
    {
        var redisPool = RedisConnectionPool.Instance;
        var connection = redisPool.GetConnection();
        try
        {
            var db = connection.GetDatabase();

            // Remove the specified key
            await db.KeyDeleteAsync(key);
        }
        catch (Exception ex)
        {
            // Log the exception for debugging
            Console.WriteLine($"Redis error on RemoveCacheAsync for key '{key}': {ex.Message}");
        }
        finally
        {
            // Ensure the connection is returned to the pool
            redisPool.ReturnConnection(connection);
        }
    }
}