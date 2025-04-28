using ExternalUserServiceLibrary.Models;
using ExternalUserServiceLibrary.Clients;
using Microsoft.Extensions.Caching.Memory;

namespace ExternalUserServiceLibrary.Services
{
    public class ExternalUserService
    {
        private readonly ExternalApiClient _apiClient;
        private readonly IMemoryCache _cache;

        public ExternalUserService(HttpClient httpClient, string baseUrl, IMemoryCache cache)
        {
            _apiClient = new ExternalApiClient(httpClient, baseUrl);
            _cache = cache;
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            // Try to get from cache first
            if (_cache.TryGetValue($"user_{userId}", out User cachedUser))
            {
                return cachedUser;
            }

            // If not in cache, call API
            var user = await _apiClient.GetUserByIdAsync(userId);

            if (user != null)
            {
                // Cache the user for future (optional: set cache expiry)
                _cache.Set($"user_{userId}", user, TimeSpan.FromMinutes(5)); // Cache for 5 minutes
            }

            return user;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            var users = new List<User>();
            int page = 1;
            ApiResponse<User> response;

            do
            {
                response = await _apiClient.GetUsersAsync(page);
                if (response?.Data != null)
                    users.AddRange(response.Data);

                page++;
            }
            while (response != null && page <= response.TotalPages);

            return users;
        }
    }
}