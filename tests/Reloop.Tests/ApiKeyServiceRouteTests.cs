using System.Net.Http;
using Reloop.Models;
using Xunit;

namespace Reloop.Tests;

public class ApiKeyServiceRouteTests
{
    private static (ReloopClient Client, MockHttpMessageHandler Handler) CreateClient()
    {
        var handler = new MockHttpMessageHandler();
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://reloop.sh")
        };

        return (new ReloopClient("rl_test", "https://reloop.sh", httpClient), handler);
    }

    [Fact]
    public async Task CreateAsync_UsesApiKeyCreateRoute()
    {
        var (client, handler) = CreateClient();

        await client.ApiKeys.CreateAsync(new CreateApiKeyParams(Name: "Production Key"));

        Assert.Equal(HttpMethod.Post, handler.LastRequest?.Method);
        Assert.Equal("/api/api-key/v1/", handler.LastRequest?.RequestUri?.PathAndQuery);
    }

    [Fact]
    public async Task PauseAsync_UsesDisableRoute()
    {
        var (client, handler) = CreateClient();

        await client.ApiKeys.PauseAsync("key_1");

        Assert.Equal(HttpMethod.Post, handler.LastRequest?.Method);
        Assert.Equal("/api/api-key/v1/disable/key_1", handler.LastRequest?.RequestUri?.PathAndQuery);
    }

    [Fact]
    public async Task RotateAsync_UsesRotateRoute()
    {
        var (client, handler) = CreateClient();

        await client.ApiKeys.RotateAsync("key_1");

        Assert.Equal(HttpMethod.Post, handler.LastRequest?.Method);
        Assert.Equal("/api/api-key/v1/rotate/key_1", handler.LastRequest?.RequestUri?.PathAndQuery);
    }
}
