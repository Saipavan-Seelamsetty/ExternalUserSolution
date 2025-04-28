using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Polly;
using Polly.Extensions.Http;
using ExternalUserServiceLibrary.Services;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Http.Headers;

// Setup Dependency Injection
var services = new ServiceCollection();

// Add MemoryCache service
services.AddMemoryCache();

services.AddHttpClient("ExternalApiClient", client =>
{
    client.BaseAddress = new Uri("https://reqres.in/api/");
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    client.DefaultRequestHeaders.Add("x-api-key", "reqres-free-v1");
})
.ConfigurePrimaryHttpMessageHandler(() =>
{
    var handler = new HttpClientHandler();
    return new PolicyHttpMessageHandler(GetRetryPolicy()) { InnerHandler = handler };
});

// Build the ServiceProvider
var serviceProvider = services.BuildServiceProvider();

// Create instances
var memoryCache = serviceProvider.GetRequiredService<IMemoryCache>();
var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();

// Get the configured HttpClient
var httpClient = httpClientFactory.CreateClient("ExternalApiClient");

// Create User Service
var userService = new ExternalUserService(httpClient, "https://reqres.in/api", memoryCache);

// Fetch all users
var users = await userService.GetAllUsersAsync();
foreach (var user in users)
{
    Console.WriteLine($"{user.Id}: {user.FirstName} {user.LastName} - {user.Email}");
}

// Fetch single user
var validUserId = users.First().Id;
var singleUser = await userService.GetUserByIdAsync(validUserId);

if (singleUser != null)
{
    Console.WriteLine($"Single User: {singleUser.FirstName} {singleUser.LastName}");
}
else
{
    Console.WriteLine("Single user not found!");
}

// --- Below define the Retry Policy ---
static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError() // Handles 5xx, 408, and network failures
        .WaitAndRetryAsync(
            retryCount: 3,
            sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)), // 2s, 4s, 8s
            onRetry: (outcome, timespan, retryAttempt, context) =>
            {
                Console.WriteLine($"Retry {retryAttempt} after {timespan.TotalSeconds} seconds due to {outcome.Exception?.Message ?? outcome.Result.StatusCode.ToString()}");
            }
        );
}