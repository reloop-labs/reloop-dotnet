using System.Collections.Generic;
using System.Net.Http;
using Reloop.Services;
using Xunit;

namespace Reloop.Tests;

public class MailServiceRouteTests
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
    public async Task SendAsync_UsesMailSendRoute()
    {
        var (client, handler) = CreateClient();

        await client.Mail.SendAsync(new Dictionary<string, object?>
        {
            ["from"] = "Reloop <hello@send.example.com>",
            ["to"] = "user@example.com",
            ["subject"] = "Welcome to Reloop",
            ["reply_to"] = "support@example.com",
        });

        Assert.Equal(HttpMethod.Post, handler.LastRequest?.Method);
        Assert.Equal("/api/mail/v1/send", handler.LastRequest?.RequestUri?.PathAndQuery);
    }
}
