using System.Net;
using System.Net.Http;
using Reloop.Exceptions;
using Reloop.Services;
using Xunit;

namespace Reloop.Tests;

public class ReloopClientTests
{
    private static (ReloopClient Client, MockHttpMessageHandler Handler) CreateClient(
        Action<MockHttpMessageHandler>? configure = null)
    {
        var handler = new MockHttpMessageHandler();
        configure?.Invoke(handler);
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://reloop.sh"),
        };

        return (new ReloopClient("rl_test", "https://reloop.sh", httpClient), handler);
    }

    [Fact]
    public void Constructor_RequiresApiKey()
    {
        Assert.Throws<ArgumentException>(() => new ReloopClient(null!));
        Assert.Throws<ArgumentException>(() => new ReloopClient("   "));
    }

    [Fact]
    public void Constructor_WiresServicesAndDefaultBaseUrl()
    {
        using var client = new ReloopClient("rl_test");

        Assert.Equal("https://reloop.sh", client.BaseUrl);
        Assert.NotNull(client.ApiKey);
        Assert.NotNull(client.Mail);
        Assert.NotNull(client.Domain);
        Assert.NotNull(client.Contacts);
        Assert.NotNull(client.Webhook);
        Assert.NotNull(client.Inbox);
        Assert.Equal("2.0.0", Version.VERSION);
    }

    [Fact]
    public void Constructor_TrimsTrailingSlashesFromBaseUrl()
    {
        using var client = new ReloopClient("rl_test", "https://reloop.sh/");

        Assert.Equal("https://reloop.sh", client.BaseUrl);
    }

    [Fact]
    public async Task FetchAsync_SetsAuthHeader()
    {
        var (client, handler) = CreateClient();
        await client.FetchAsync<object>(HttpMethod.Get, "/v1/test");

        Assert.Equal("rl_test", handler.LastRequest?.Headers.GetValues("x-api-key").Single());
    }

    [Fact]
    public async Task FetchAsync_MapsApiErrorToReloopApiException()
    {
        var (client, _) = CreateClient(handler =>
        {
            handler.ResponseStatusCode = HttpStatusCode.Unauthorized;
            handler.ResponseBody = "{\"message\":\"bad key\"}";
        });

        var err = await Assert.ThrowsAsync<ReloopApiException>(
            () => client.FetchAsync<object>(HttpMethod.Get, "/v1/test"));

        Assert.Equal(401, err.Status);
        Assert.Equal("bad key", err.Body.Message);
    }

    [Fact]
    public async Task FetchAsync_NetworkErrorBecomesReloopApiException()
    {
        using var client = new ReloopClient("rl_test", "http://127.0.0.1:1");

        var err = await Assert.ThrowsAsync<ReloopApiException>(
            () => client.FetchAsync<object>(HttpMethod.Get, "/v1/test"));

        Assert.Equal(0, err.Status);
        Assert.Contains("network", err.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task FetchAsync_MalformedSuccessJsonBecomesReloopApiException()
    {
        var (client, _) = CreateClient(handler =>
        {
            handler.ResponseBody = "{not-json";
        });

        var err = await Assert.ThrowsAsync<ReloopApiException>(
            () => client.FetchAsync<object>(HttpMethod.Get, "/v1/test"));

        Assert.Contains("parsing", err.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Constructor_DoesNotMutateSharedHttpClientDefaults()
    {
        var handler = new MockHttpMessageHandler();
        var shared = new HttpClient(handler) { BaseAddress = new Uri("https://reloop.sh") };
        using var clientA = new ReloopClient("rl_a", "https://reloop.sh", shared);
        using var clientB = new ReloopClient("rl_b", "https://reloop.sh", shared);

        await clientA.FetchAsync<object>(HttpMethod.Get, "/v1/a");
        Assert.Equal("rl_a", handler.LastRequest!.Headers.GetValues("x-api-key").Single());

        await clientB.FetchAsync<object>(HttpMethod.Get, "/v1/b");
        Assert.Equal("rl_b", handler.LastRequest!.Headers.GetValues("x-api-key").Single());

        Assert.False(shared.DefaultRequestHeaders.Contains("x-api-key"));
    }

    [Fact]
    public async Task FetchAsync_RejectsAbsoluteUrls()
    {
        var (client, handler) = CreateClient();

        await Assert.ThrowsAsync<ArgumentException>(
            () => client.FetchAsync<object>(HttpMethod.Get, "https://evil.example/steal"));
        await Assert.ThrowsAsync<ArgumentException>(
            () => client.FetchAsync<object>(HttpMethod.Get, "//evil.example/steal"));

        Assert.Equal(0, handler.RequestCount);
    }
}
