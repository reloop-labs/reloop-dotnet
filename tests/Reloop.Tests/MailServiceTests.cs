using Reloop.Exceptions;
using Reloop.Models;
using Reloop.Services;
using Xunit;
using static Reloop.Models.MailModels;

namespace Reloop.Tests;

public class MailServiceTests
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
    public async Task SendAsync_HappyPath()
    {
        var (client, handler) = CreateClient(h =>
        {
            h.ResponseBody =
                "{\"success\":true,\"messageId\":\"msg_1\",\"status\":\"queued\",\"timestamp\":\"t\",\"id\":\"em_1\"}";
        });

        var response = await client.Mail.SendAsync(new SendMailParams
        {
            From = "a@example.com",
            To = "b@example.com",
            Subject = "Hi",
        });

        Assert.Equal(HttpMethod.Post, handler.LastRequest?.Method);
        Assert.Equal("/api/mail/v1/send", handler.LastRequest?.RequestUri?.PathAndQuery);
        Assert.Equal("rl_test", handler.LastRequest?.Headers.GetValues("x-api-key").Single());
        Assert.Contains("\"from\":\"a@example.com\"", await handler.LastRequest!.Content!.ReadAsStringAsync());
        Assert.NotNull(response);
        Assert.True(response!.Success);
        Assert.Equal("em_1", response.Id);
    }

    [Fact]
    public async Task SendAsync_ApiError()
    {
        var (client, _) = CreateClient(h =>
        {
            h.ResponseStatusCode = System.Net.HttpStatusCode.BadRequest;
            h.ResponseBody = "{\"message\":\"invalid\"}";
        });

        await Assert.ThrowsAsync<ReloopApiException>(() => client.Mail.SendAsync(new SendMailParams
        {
            From = "a@x.com",
            To = "b@x.com",
            Subject = "Hi",
        }));
    }

    [Fact]
    public async Task SendAsync_ValidationFailureDoesNotCallHttp()
    {
        var (client, handler) = CreateClient();

        var err = await Assert.ThrowsAsync<ReloopValidationException>(() => client.Mail.SendAsync(new SendMailParams
        {
            From = "",
            To = "b@x.com",
            Subject = "Hi",
        }));

        Assert.Equal("from", err.Field);
        Assert.Equal(0, handler.RequestCount);
    }

    [Fact]
    public void SurfaceLock()
    {
        var methods = typeof(MailService)
            .GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
            .Where(m => m.DeclaringType == typeof(MailService))
            .Select(m => m.Name)
            .ToHashSet();

        Assert.Equal(new HashSet<string> { "SendAsync" }, methods);
    }
}
