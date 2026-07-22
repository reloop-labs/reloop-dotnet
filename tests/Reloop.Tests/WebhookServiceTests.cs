using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Reloop.Exceptions;
using Reloop.Models;
using Reloop.Services;
using Xunit;
using static Reloop.Models.WebhookModels;

namespace Reloop.Tests;

public class WebhookServiceTests
{
    private const string Secret = "whsec_test_secret";
    private const string Payload =
        "{\"id\":\"evt_123456789\",\"event\":\"domain.created\",\"payload\":{\"domainId\":\"dom_1\"},\"timestamp\":1735689600}";

    private static (ReloopClient Client, MockHttpMessageHandler Handler) CreateClient(
        Action<MockHttpMessageHandler>? configure = null)
    {
        var handler = new MockHttpMessageHandler();
        configure?.Invoke(handler);
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://reloop.sh") };
        return (new ReloopClient("rl_test", "https://reloop.sh", httpClient), handler);
    }

    [Fact]
    public async Task CreateAsync_HappyPath()
    {
        var (client, handler) = CreateClient(h =>
        {
            h.ResponseBody =
                "{\"id\":\"wh_123456789\",\"name\":\"Production webhook\",\"url\":\"https://example.com/webhooks/reloop\",\"secret\":\"whsec_test\",\"status\":\"active\",\"rateLimitEnabled\":false,\"maxRequestsPerMinute\":60,\"maxRetries\":3,\"retryBackoffMultiplier\":2,\"successCount\":0,\"failureCount\":0,\"consecutiveFailures\":0,\"events\":[\"domain.created\"],\"createdAt\":\"2026-01-01T00:00:00.000Z\",\"updatedAt\":\"2026-01-01T00:00:00.000Z\"}";
        });

        var response = await client.Webhook.CreateAsync(new CreateWebhookParams(
            "Production webhook",
            "https://example.com/webhooks/reloop",
            new List<string> { "domain.created" }));

        Assert.Equal(HttpMethod.Post, handler.LastRequest?.Method);
        Assert.Equal("/api/webhook/v1/", handler.LastRequest?.RequestUri?.PathAndQuery);
        Assert.Equal("wh_123456789", response!.Id);
    }

    [Fact]
    public async Task Validation_NoHttp()
    {
        var (client, handler) = CreateClient();
        await Assert.ThrowsAsync<ReloopValidationException>(() => client.Webhook.GetAsync(""));
        await Assert.ThrowsAsync<ReloopValidationException>(
            () => client.Webhook.UpdateAsync("wh_1", new UpdateWebhookParams()));
        await Assert.ThrowsAsync<ReloopValidationException>(() => client.Webhook.TriggerAsync(new TriggerWebhookParams()));
        await Assert.ThrowsAsync<ReloopValidationException>(
            () => client.Webhook.CreateAsync(new CreateWebhookParams()));
        await Assert.ThrowsAsync<ReloopValidationException>(
            () => client.Webhook.CreateAsync(new CreateWebhookParams
            {
                Url = "https://example.com/hook",
                Events = new List<string>(),
            }));
        await Assert.ThrowsAsync<ReloopValidationException>(
            () => client.Webhook.ListDeliveriesAsync("wh_1", new ListWebhookDeliveriesParams { Status = "" }));
        Assert.Equal(0, handler.RequestCount);
    }

    [Fact]
    public async Task PauseAsync_PatchesPausedStatus()
    {
        var (client, handler) = CreateClient();
        await client.Webhook.PauseAsync("wh_1");
        Assert.Equal(new HttpMethod("PATCH"), handler.LastRequest?.Method);
        Assert.Contains("\"status\":\"paused\"", handler.LastRequestBody);
    }

    [Fact]
    public void Verify_AcceptsValidSignature()
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
        var header = SignPayload(Secret, timestamp, Payload);
        var parameters = new VerifyWebhookParams
        {
            Payload = Encoding.UTF8.GetBytes(Payload),
            Headers = new Dictionary<string, string> { [WebhookVerify.WebhookSignatureHeader] = header },
            Secret = Secret,
        };

        var webhookEvent = WebhookVerify.Verify(parameters);
        Assert.Equal("evt_123456789", webhookEvent.Id);
        Assert.Equal("domain.created", webhookEvent.Event);
        Assert.Equal("dom_1", GetPayloadString(webhookEvent.Payload!, "domainId"));
    }

    [Fact]
    public void ConstructEvent_VerifiesSignature()
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
        var header = SignPayload(Secret, timestamp, Payload);
        var webhookEvent = WebhookService.ConstructEvent(Encoding.UTF8.GetBytes(Payload), header, Secret, 300);
        Assert.Equal("domain.created", webhookEvent.Event);
    }

    [Fact]
    public void Verify_RejectsWrongSecret()
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
        var header = SignPayload(Secret, timestamp, Payload);
        var parameters = new VerifyWebhookParams
        {
            Payload = Encoding.UTF8.GetBytes(Payload),
            Headers = new Dictionary<string, string> { [WebhookVerify.WebhookSignatureHeader] = header },
            Secret = "wrong_secret",
        };

        Assert.Throws<WebhookSignatureException>(() => WebhookVerify.Verify(parameters));
    }

    [Fact]
    public void Verify_RejectsExpiredTimestamp()
    {
        var timestamp = (DateTimeOffset.UtcNow.ToUnixTimeSeconds() - 600).ToString();
        var header = SignPayload(Secret, timestamp, Payload);
        var parameters = new VerifyWebhookParams
        {
            Payload = Encoding.UTF8.GetBytes(Payload),
            Headers = new Dictionary<string, string> { [WebhookVerify.WebhookSignatureHeader] = header },
            Secret = Secret,
            Tolerance = 300,
        };

        var err = Assert.Throws<WebhookSignatureException>(() => WebhookVerify.Verify(parameters));
        Assert.Contains("tolerance", err.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Verify_RejectsNegativeTolerance()
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
        var header = SignPayload(Secret, timestamp, Payload);
        var parameters = new VerifyWebhookParams
        {
            Payload = Encoding.UTF8.GetBytes(Payload),
            Headers = new Dictionary<string, string> { [WebhookVerify.WebhookSignatureHeader] = header },
            Secret = Secret,
            Tolerance = -1,
        };

        var err = Assert.Throws<WebhookSignatureException>(() => WebhookVerify.Verify(parameters));
        Assert.Contains("non-negative", err.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void SurfaceLock()
    {
        var instanceMethods = typeof(WebhookService)
            .GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
            .Where(m => m.DeclaringType == typeof(WebhookService))
            .Select(m => m.Name)
            .ToHashSet();

        Assert.Equal(
            new HashSet<string>
            {
                "CreateAsync", "DeleteAsync", "DisableAsync", "EnableAsync", "GetAsync", "ListAsync",
                "ListDeliveriesAsync", "PauseAsync", "RetryDeliveryAsync", "TriggerAsync", "UpdateAsync", "Verify",
            },
            instanceMethods);

        Assert.NotNull(typeof(WebhookService).GetMethod("ConstructEvent"));
    }

    private static string SignPayload(string secret, string timestamp, string payload)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(timestamp + "." + payload));
        return "t=" + timestamp + ",v1=" + BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
    }

    private static string? GetPayloadString(Dictionary<string, object> payload, string key)
    {
        if (!payload.TryGetValue(key, out var value))
        {
            return null;
        }

        return value is JsonElement element ? element.GetString() : value?.ToString();
    }
}
