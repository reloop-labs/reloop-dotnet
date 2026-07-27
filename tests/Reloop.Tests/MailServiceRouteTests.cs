using Reloop.Models;
using Xunit;
using static Reloop.Models.MailModels;

namespace Reloop.Tests;

public class MailServiceRouteTests
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
    public async Task SendAsync_UsesMailSendRoute()
    {
        var (client, handler) = CreateClient();

        await client.Mail.SendAsync(new SendMailParams
        {
            From = "Reloop <hello@send.example.com>",
            To = "user@example.com",
            Subject = "Welcome to Reloop",
            ReplyTo = "support@example.com",
        });

        Assert.Equal(HttpMethod.Post, handler.LastRequest?.Method);
        Assert.Equal("/api/mail/v1/send", handler.LastRequest?.RequestUri?.PathAndQuery);
    }
}
