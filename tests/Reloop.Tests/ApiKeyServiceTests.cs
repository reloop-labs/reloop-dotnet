using System.Net;
using System.Net.Http;
using System.Text.Json;
using Reloop.Exceptions;
using Reloop.Models;
using Reloop.Services;
using Xunit;
using static Reloop.Models.ApiKeyModels;

namespace Reloop.Tests;

public class ApiKeyServiceTests
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
    public void CreateApiKeyParams_SerializesNameOnly()
    {
        var json = JsonSerializer.Serialize(new CreateApiKeyParams("Production Key"));

        Assert.Contains("\"name\":\"Production Key\"", json);
        Assert.DoesNotContain("enabled", json);
    }

    [Fact]
    public async Task CreateAsync_HappyPath()
    {
        var (client, handler) = CreateClient(h =>
        {
            h.ResponseBody = "{\"id\":\"key_1\",\"key\":\"rl_secret\",\"object\":\"api_key\"}";
        });

        var response = await client.ApiKey.CreateAsync(new CreateApiKeyParams("prod"));

        Assert.Equal(HttpMethod.Post, handler.LastRequest?.Method);
        Assert.Equal("/api/api-key/v1/", handler.LastRequest?.RequestUri?.PathAndQuery);
        Assert.Contains("\"name\":\"prod\"", await handler.LastRequest!.Content!.ReadAsStringAsync());
        Assert.Equal("rl_secret", response!.Key);
    }

    [Fact]
    public async Task CreateAsync_ValidationNoHttp()
    {
        var (client, handler) = CreateClient();

        await Assert.ThrowsAsync<ReloopValidationException>(
            () => client.ApiKey.CreateAsync(new CreateApiKeyParams("")));

        Assert.Equal(0, handler.RequestCount);
    }

    [Fact]
    public async Task GetAsync_ApiError()
    {
        var (client, _) = CreateClient(h =>
        {
            h.ResponseStatusCode = HttpStatusCode.InternalServerError;
            h.ResponseBody = "{\"message\":\"boom\"}";
        });

        await Assert.ThrowsAsync<ReloopApiException>(() => client.ApiKey.GetAsync("key_1"));
    }

    [Fact]
    public void SurfaceLock()
    {
        var methods = typeof(ApiKeyService)
            .GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
            .Where(m => m.DeclaringType == typeof(ApiKeyService))
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
                "RotateAsync",
                "EnableAsync",
                "DisableAsync",
            },
            methods);
    }

    [Fact]
    public void PauseAsync_DoesNotExist()
    {
        Assert.Null(typeof(ApiKeyService).GetMethod("PauseAsync"));
    }
}
