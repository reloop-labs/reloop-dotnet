using System.Net;
using System.Net.Http;
using System.Text.Json;
using Reloop.Exceptions;
using Reloop.Models;
using Reloop.Services;
using Xunit;
using static Reloop.Models.DomainModels;

namespace Reloop.Tests;

public class DomainServiceTests
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
    public void CreateDomainParams_SerializesWithSnakeCase()
    {
        var json = JsonSerializer.Serialize(new CreateDomainParams("send.example.com")
        {
            ClickTracking = true,
        });

        Assert.Contains("\"click_tracking\":true", json);
        Assert.Contains("\"domain\":\"send.example.com\"", json);
        Assert.DoesNotContain("clickTracking", json);
        Assert.DoesNotContain("custom_return_path", json);
    }

    [Fact]
    public void UpdateDomainParams_SerializesWithSnakeCase()
    {
        var json = JsonSerializer.Serialize(new UpdateDomainParams
        {
            ClickTracking = false,
            OpenTracking = true,
            Tls = "enforced",
        });

        Assert.Contains("\"click_tracking\":false", json);
        Assert.DoesNotContain("clickTracking", json);
    }

    [Fact]
    public async Task CreateAsync_HappyPath()
    {
        var (client, handler) = CreateClient(h =>
        {
            h.ResponseBody = "{\"id\":\"dom_1\",\"domain\":\"example.com\",\"object\":\"domain\"}";
        });

        var response = await client.Domain.CreateAsync(new CreateDomainParams("example.com"));

        Assert.Equal(HttpMethod.Post, handler.LastRequest?.Method);
        Assert.Equal("/api/domain/v1/create", handler.LastRequest?.RequestUri?.PathAndQuery);
        Assert.Contains("\"domain\":\"example.com\"", await handler.LastRequest!.Content!.ReadAsStringAsync());
        Assert.Equal("dom_1", response!.Id);
    }

    [Fact]
    public async Task GetAsync_ValidationNoHttp()
    {
        var (client, handler) = CreateClient();

        await Assert.ThrowsAsync<ReloopValidationException>(() => client.Domain.GetAsync("  "));

        Assert.Equal(0, handler.RequestCount);
    }

    [Fact]
    public async Task GetAsync_ApiError()
    {
        var (client, _) = CreateClient(h =>
        {
            h.ResponseStatusCode = HttpStatusCode.NotFound;
            h.ResponseBody = "{\"message\":\"missing\"}";
        });

        await Assert.ThrowsAsync<ReloopApiException>(() => client.Domain.GetAsync("dom_missing"));
    }

    [Fact]
    public void SurfaceLock()
    {
        var methods = typeof(DomainService)
            .GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
            .Where(m => m.DeclaringType == typeof(DomainService))
            .Select(m => m.Name)
            .ToHashSet();

        Assert.Equal(
            new HashSet<string>
            {
                "CreateAsync",
                "ListAsync",
                "GetAsync",
                "UpdateAsync",
                "DeleteAsync",
                "VerifyAsync",
            },
            methods);
    }

    [Fact]
    public void LegacyMethods_DoNotExist()
    {
        Assert.Null(typeof(DomainService).GetMethod("GetNameserversAsync"));
        Assert.Null(typeof(DomainService).GetMethod("ForwardDnsAsync"));
    }
}
