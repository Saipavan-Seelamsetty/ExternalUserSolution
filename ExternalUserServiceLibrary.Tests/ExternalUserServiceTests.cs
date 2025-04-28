using ExternalUserServiceLibrary.Services;
using RichardSzalay.MockHttp;
using Microsoft.Extensions.Caching.Memory;

namespace ExternalUserServiceLibrary.Tests
{
    public class ExternalUserServiceTests
    {
        private const string BaseUrl = "https://reqres.in/api";

        [Fact]
        public async Task GetUserByIdAsync_ReturnsUser_WhenUserExists()
        {
            // Arrange
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When($"{BaseUrl}/users/2")
                .Respond("application/json", "{\"data\":{\"id\":2,\"email\":\"janet.weaver@reqres.in\",\"first_name\":\"Janet\",\"last_name\":\"Weaver\",\"avatar\":\"avatar_url\"}}");

            var client = new HttpClient(mockHttp);

            //Create a MemoryCache instance
            var memoryCache = new MemoryCache(new MemoryCacheOptions());

            //Pass MemoryCache to service
            var service = new ExternalUserService(client, BaseUrl, memoryCache);

            // Act
            var user = await service.GetUserByIdAsync(2);

            // Assert
            Assert.NotNull(user);
            Assert.Equal(2, user.Id);
            Assert.Equal("Janet", user.FirstName);
        }
    }
}