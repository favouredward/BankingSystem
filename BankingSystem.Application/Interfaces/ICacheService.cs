using System;
using System.Threading.Tasks;

namespace BankingSystem.Application.Interfaces
{
    /// <summary>
    /// Abstraction for cache operations using Redis.
    /// </summary>
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T value, TimeSpan? expiry = null);
        Task RemoveAsync(string key);
    }
}
