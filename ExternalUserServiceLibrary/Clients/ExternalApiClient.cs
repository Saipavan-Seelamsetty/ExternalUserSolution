using ExternalUserServiceLibrary.Models;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ExternalUserServiceLibrary.Clients
{
    internal class ExternalApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public ExternalApiClient(HttpClient httpClient, string baseUrl)
        {
            _httpClient = httpClient;
            _baseUrl = baseUrl.TrimEnd('/');
        }

        public async Task<ApiResponse<User>> GetUsersAsync(int pageNumber)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/users?page={pageNumber}");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ApiResponse<User>>(json);
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/users/{userId}");

            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var userWrapper = JsonSerializer.Deserialize<UserWrapper>(json);
            return userWrapper.Data;
        }

        private class UserWrapper
        {
            [JsonPropertyName("data")]
            public User Data { get; set; }

            [JsonPropertyName("support")]
            public Support Support { get; set; }
        }
    }
}