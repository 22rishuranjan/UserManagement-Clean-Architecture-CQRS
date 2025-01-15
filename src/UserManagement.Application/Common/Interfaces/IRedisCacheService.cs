
namespace UserManagement.Application.Common.Interfaces;

public interface IRedisCacheService
{

    public Task<T> GetOrSetCacheValueAsync<T>(string key, Func<Task<T>> fetchDataAsync);

    Task RemoveCacheAsync(string key);

}
