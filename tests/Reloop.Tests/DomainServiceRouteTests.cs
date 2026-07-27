using System.Net.Http;
using Reloop.Models;
using Xunit;
using static Reloop.Models.DomainModels;

namespace Reloop.Tests;

public class DomainServiceRouteTests
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
    public async Task CreateAsync_UsesDomainCreateRoute()
    {
        var (client, handler) = CreateClient();

        await client.Domain.CreateAsync(new CreateDomainParams("send.example.com")
        {
            ClickTracking = true,
        });

        Assert.Equal(HttpMethod.Post, handler.LastRequest?.Method);
        Assert.Equal("/api/domain/v1/create", handler.LastRequest?.RequestUri?.PathAndQuery);
    }

    [Fact]
    public async Task ListAsync_UsesListRouteWithQuery()
    {
        var (client, handler) = CreateClient();

        await client.Domain.ListAsync(new ListDomainsParams
        {
            Page = 2,
            Limit = 5,
            Q = "example",
            Status = "active",
        });

        Assert.Equal(HttpMethod.Get, handler.LastRequest?.Method);
        Assert.Equal(
            "/api/domain/v1/list?page=2&limit=5&q=example&status=active",
            handler.LastRequest?.RequestUri?.PathAndQuery);
    }

    [Fact]
    public async Task VerifyAsync_UsesVerifyRoute()
    {
        var (client, handler) = CreateClient();

        await client.Domain.VerifyAsync("dom_1");

        Assert.Equal(HttpMethod.Post, handler.LastRequest?.Method);
        Assert.Equal("/api/domain/v1/verify/dom_1", handler.LastRequest?.RequestUri?.PathAndQuery);
    }

    [Fact]
    public async Task DeleteAsync_UsesDeleteRoute()
    {
        var (client, handler) = CreateClient();

        await client.Domain.DeleteAsync("dom_1");

        Assert.Equal(HttpMethod.Delete, handler.LastRequest?.Method);
        Assert.Equal("/api/domain/v1/dom_1", handler.LastRequest?.RequestUri?.PathAndQuery);
    }
}
