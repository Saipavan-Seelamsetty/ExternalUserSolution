# ExternalUserServiceSolution

## Overview

This project demonstrates a .NET Core component that interacts with a public API ([reqres.in](https://reqres.in)) to fetch, cache, and handle user data robustly.

It is designed to simulate how a backend service integrates with external systems while ensuring resilience and performance.

The solution includes:
- A class library (`ExternalUserServiceLibrary`) containing the core logic.
- A unit test project (`ExternalUserServiceLibrary.Tests`) validating the service functionality.
- A console app (`ExternalUserServiceConsoleDemo`) to demonstrate usage.

---

## Project Structure

```
ExternalUserSolution/
 ├── ExternalUserServiceLibrary/
 │    ├── Clients/ExternalApiClient.cs
 │    ├── Models/ (User.cs, ApiResponse.cs, etc.)
 │    └── Services/ExternalUserService.cs
 │
 ├── ExternalUserServiceLibrary.Tests/
 │    └── ExternalUserServiceTests.cs
 │
 └── ExternalUserServiceConsoleDemo/
      └── Program.cs
```

---

## Core Features

### ✅ HttpClient with Retry Logic
- API requests are retried automatically up to 3 times for transient failures (e.g., network errors, 5xx HTTP errors).
- Implemented using **Polly** and **Polly.Extensions.Http**.

### ✅ Memory Caching
- Single user details (`GetUserByIdAsync`) are cached using **IMemoryCache** for 5 minutes.
- Reduces unnecessary API calls and improves performance.

### ✅ Proper Async/Await Usage
- All API calls are fully asynchronous for scalability.

### ✅ Error Handling
- Handles API errors gracefully (e.g., 404 Not Found returns `null`).
- Catches network issues with retry policies.

### ✅ Configuration Driven
- Base URL (`https://reqres.in/api/`) and API Key are configurable.

---

## How to Build and Run

### 1. Prerequisites

- Visual Studio 2022+ or VS Code
- .NET 6 SDK (or later)

### 2. Clone the Repository

```bash
git clone <your-repo-url>
cd ExternalUserSolution
```

### 3. Build the Solution

Open the solution in Visual Studio and run:
- **Build ➔ Rebuild Solution**

Or using CLI:

```bash
dotnet build
```

### 4. Run the Console Application

Set `ExternalUserServiceConsoleDemo` as the Startup Project, then:

- Press **F5** or **Ctrl+F5**.

You should see output listing:
- All Users
- A single user details
- (Retry attempts if any transient failures happen)

---

## How to Test

- Open Test Explorer in Visual Studio.
- Run all tests inside `ExternalUserServiceLibrary.Tests`.

Unit tests cover:
- Fetching single user correctly
- Handling network scenarios using MockHttp
- Validating caching behavior

---

## Design Decisions

- **Separation of Concerns**: `ExternalApiClient` is responsible for low-level HTTP calls. `ExternalUserService` handles business logic.
- **Dependency Injection**: All dependencies (`HttpClient`, `IMemoryCache`) are injected for testability.
- **Resilience**: Implemented retry mechanism to handle transient faults gracefully.
- **Caching**: Reduced load on the external API using `IMemoryCache` caching strategy.

---

## Future Improvements

- Add Pagination support for large user lists.
- Implement distributed caching (e.g., Redis) for scaling across multiple servers.
- Extend Retry policy with Circuit Breaker pattern.
- Add full logging using ILogger.
- Move configuration (API base URL, API key) fully into `appsettings.json`.

---

## Libraries Used

| Library | Purpose |
|:---|:---|
| Microsoft.Extensions.Http | HttpClientFactory and DI setup |
| Microsoft.Extensions.Caching.Memory | Memory caching |
| Polly | Retry policies |
| Polly.Extensions.Http | Retry policies integrated with HttpClient |
| RichardSzalay.MockHttp | Mocking HTTP calls in unit tests |

---

## Author

Saipavan Seelamsetty

