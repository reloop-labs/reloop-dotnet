using System.Net.Http;
using Reloop.Models;
using Reloop.Services;
using Xunit;

namespace Reloop.Tests;

public class DomainServiceRouteTests
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
    public async Task CreateAsync_UsesDomainCreateRoute()
    {
        var (client, handler) = CreateClient();

        await client.Domain.CreateAsync(new CreateDomainParams(
            Domain: "send.example.com",
            ClickTracking: true,
            CustomReturnPath: "inbound"));

        Assert.Equal(HttpMethod.Post, handler.LastRequest?.Method);
        Assert.Equal("/api/domain/v1/create", handler.LastRequest?.RequestUri?.PathAndQuery);
    }

    [Fact]
    public async Task GetNameserversAsync_UsesNameserversRoute()
    {
        var (client, handler) = CreateClient();

        await client.Domain.GetNameserversAsync("dom_1");

        Assert.Equal(HttpMethod.Get, handler.LastRequest?.Method);
        Assert.Equal("/api/domain/v1/nameservers/dom_1", handler.LastRequest?.RequestUri?.PathAndQuery);
    }

    [Fact]
    public async Task ForwardDnsAsync_UsesForwardDnsRoute()
    {
        var (client, handler) = CreateClient();

        await client.Domain.ForwardDnsAsync("dom_1", new ForwardDnsParams(Email: "admin@example.com"));

        Assert.Equal(HttpMethod.Post, handler.LastRequest?.Method);
        Assert.Equal("/api/domain/v1/verify/dom_1/forward-dns", handler.LastRequest?.RequestUri?.PathAndQuery);
    }

    [Fact]
    public void BuildListQuery_IncludesFilters()
    {
        var query = DomainService.BuildListQuery(new ListDomainsParams
        {
            Page = 2,
            Limit = 5,
            Q = "example",
            Status = "active",
        });

        Assert.Equal("page=2&limit=5&q=example&status=active", query);
    }
}
