using System.Net.Http;
using Reloop.Models;
using Xunit;
using static Reloop.Models.ApiKeyModels;

namespace Reloop.Tests;

public class ApiKeyServiceRouteTests
{
    private static (ReloopClient Client, MockHttpMessageHandler Handler) CreateClient()
    {
        var handler = new MockHttpMessageHandler();
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://reloop.sh"),
        };

        return (new ReloopClient("rl_test", "https://reloop.sh", httpClient), handler);
    }

    [Fact]
    public async Task CreateAsync_UsesApiKeyCreateRoute()
    {
        var (client, handler) = CreateClient();

        await client.ApiKey.CreateAsync(new CreateApiKeyParams("Production Key"));

        Assert.Equal(HttpMethod.Post, handler.LastRequest?.Method);
        Assert.Equal("/api/api-key/v1/", handler.LastRequest?.RequestUri?.PathAndQuery);
    }

    [Fact]
    public async Task DisableAsync_UsesDisableRoute()
    {
        var (client, handler) = CreateClient();

        await client.ApiKey.DisableAsync("key_1");

        Assert.Equal(HttpMethod.Post, handler.LastRequest?.Method);
        Assert.Equal("/api/api-key/v1/disable/key_1", handler.LastRequest?.RequestUri?.PathAndQuery);
    }

    [Fact]
    public async Task RotateAsync_UsesRotateRoute()
    {
        var (client, handler) = CreateClient();

        await client.ApiKey.RotateAsync("key_1");

        Assert.Equal(HttpMethod.Post, handler.LastRequest?.Method);
        Assert.Equal("/api/api-key/v1/rotate/key_1", handler.LastRequest?.RequestUri?.PathAndQuery);
    }

    [Fact]
    public async Task ListAsync_UsesQueryParams()
    {
        var (client, handler) = CreateClient();

        await client.ApiKey.ListAsync(new ApiKeyListParams
        {
            Page = 2,
            Limit = 10,
            Enabled = true,
            Q = "prod",
        });

        Assert.Equal(HttpMethod.Get, handler.LastRequest?.Method);
        Assert.Equal(
            "/api/api-key/v1/?page=2&limit=10&enabled=true&q=prod",
            handler.LastRequest?.RequestUri?.PathAndQuery);
    }
}
